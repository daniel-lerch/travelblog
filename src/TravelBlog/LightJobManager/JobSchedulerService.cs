﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TravelBlog.LightJobManager
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

        public async Task Enqueue(TData data, TContext context)
        {
            // Enqueueing new work items is possible at all times. Scaffolding will lock and validate the state before executing.

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
            using IServiceScope scope = serviceProvider.CreateScope();
            TContext context = ActivatorUtilities.CreateInstance<TContext>(serviceProvider);

            while (true)
            {
                IReadOnlyCollection<TData> jobs;
                using (await runnerLock.LockAsync())
                {
                    jobs = await context.GetJobs();
                    if (jobs.Count == 0)
                    {
                        runnerActive = false;
                        return;
                    }
                }

                foreach (TData job in jobs)
                {
                    for (bool success = false; !success; await Task.Delay(TimeSpan.FromMinutes(5)))
                    {
                        lock (this)
                        {
                            if (!serviceRunning) return;
                            jobTask = ExecuteAndRemoveJob(job, context);
                        }
                        success = await jobTask;
                        lock (this)
                        {
                            jobTask = null;
                        }
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
