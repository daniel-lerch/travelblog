using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TravelBlog.Services.LightJobManager
{
    /// <summary>
    /// Generic reliable job scheduler for serial tasks with retry. This service is not safe to restart after stopping it.
    /// </summary>
    /// <typeparam name="TData">Persistent data for job.</typeparam>
    /// <typeparam name="TContext">Dynamically created context which will be created using dependency injection.</typeparam>
    public class JobSchedulerService<TData, TContext> : IHostedService where TContext : IJobContext<TData>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly AsyncLock runnerLock;

        private bool serviceRunning;
        private bool runnerActive;
        private Task<bool>? jobTask;

        public JobSchedulerService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            runnerLock = new AsyncLock();
        }

        public async Task Enqueue(TData data)
        {
            // Enqueueing new work items is possible at all times. Scaffolding will lock and validate the state before executing.

            using IServiceScope scope = serviceProvider.CreateScope();
            var context = ActivatorUtilities.CreateInstance<TContext>(scope.ServiceProvider);
            using (await runnerLock.LockAsync())
            {
                await context.Add(data);
                if (!runnerActive)
                {
                    runnerActive = true;
                    ThreadPool.QueueUserWorkItem(StartRunner);
                }
            }
        }

        public async Task Enqueue(IEnumerable<TData> data)
        {
            // Enqueueing new work items is possible at all times. Scaffolding will lock and validate the state before executing.

            using IServiceScope scope = serviceProvider.CreateScope();
            var context = ActivatorUtilities.CreateInstance<TContext>(scope.ServiceProvider);
            using (await runnerLock.LockAsync())
            {
                await context.AddRange(data);
                if (!runnerActive)
                {
                    runnerActive = true;
                    ThreadPool.QueueUserWorkItem(StartRunner);
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            serviceRunning = true;
            runnerActive = true;
            ThreadPool.QueueUserWorkItem(StartRunner);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Task? runningJobTask;
            lock (this)
            {
                serviceRunning = false;
                runningJobTask = jobTask;
            }

            if (runningJobTask != null) await runningJobTask;
        }

        private async void StartRunner(object? _)
        {
            while (!await AttemptExecution())
            {
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }

        private async Task<bool> AttemptExecution()
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            var context = ActivatorUtilities.CreateInstance<TContext>(scope.ServiceProvider);

            while (true)
            {
                IReadOnlyCollection<TData> jobs;
                using (await runnerLock.LockAsync())
                {
                    jobs = await context.GetJobs();
                    if (jobs.Count == 0)
                    {
                        runnerActive = false;
                        return true;
                    }
                }

                foreach (TData job in jobs)
                {
                    while (true)
                    {
                        lock (this)
                        {
                            if (!serviceRunning) return true;
                            jobTask = ExecuteAndRemoveJob(job, context);
                        }
                        bool success = await jobTask;
                        lock (this)
                        {
                            jobTask = null;
                        }
                        if (success) break;
                        // Indicate runner to pause for a timeout and execute again
                        return false;
                    }
                }
            }
        }

        private static async Task<bool> ExecuteAndRemoveJob(TData data, TContext context)
        {
            bool success = await context.Execute(data);
            if (success) await context.Remove(data);
            return success;
        }
    }
}
