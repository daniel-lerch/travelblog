using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Threading.Tasks;
using TravelBlog.Services.LightJobManager;

namespace TravelBlog.Tests
{
    [TestClass]
    public class JobSchedulerServiceTests
    {
        [TestMethod]
        public async Task TestStart()
        {
            var context = new FakeJobContext();
            var data = new TaskCompletionSource<bool>();
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var jobScheduler = new JobSchedulerService<TaskCompletionSource<bool>, FakeJobContext>(serviceProvider);

            await jobScheduler.StartAsync(default);
            await jobScheduler.Enqueue(data);
            CollectionAssert.AreEqual(new[] { data }, (ICollection)await context.GetJobs());
            data.SetResult(true);
            await Task.Delay(50);
            Assert.AreEqual(0, (await context.GetJobs()).Count);
            await jobScheduler.StopAsync(default);
        }

        [TestMethod]
        public async Task TestStop()
        {
            var context = new FakeJobContext();
            var data = new TaskCompletionSource<bool>();
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var jobScheduler = new JobSchedulerService<TaskCompletionSource<bool>, FakeJobContext>(serviceProvider);

            await jobScheduler.StartAsync(default);
            await jobScheduler.Enqueue(data);
            CollectionAssert.AreEqual(new[] { data }, (ICollection)await context.GetJobs());
            await Task.Delay(50);
            Task stop = jobScheduler.StopAsync(default);
            await Task.Delay(50);
            Assert.IsFalse(stop.IsCompleted);
            data.SetResult(true);
            await stop;
            Assert.AreEqual(0, (await context.GetJobs()).Count);
        }

        [TestMethod]
        public async Task TestStartMultiple()
        {
            var context = new FakeJobContext();
            var data = new[] { new TaskCompletionSource<bool>(), new TaskCompletionSource<bool>(), new TaskCompletionSource<bool>() };
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var jobScheduler = new JobSchedulerService<TaskCompletionSource<bool>, FakeJobContext>(serviceProvider);

            await jobScheduler.StartAsync(default);
            await jobScheduler.Enqueue(data);
            CollectionAssert.AreEqual(data, (ICollection)await context.GetJobs());
            for (int i = 0; i < data.Length; i++)
            {
                data[i].SetResult(true);
                await Task.Delay(50);
                Assert.AreEqual(data.Length - i - 1, (await context.GetJobs()).Count);
            }
            await jobScheduler.StopAsync(default);
        }
    }
}
