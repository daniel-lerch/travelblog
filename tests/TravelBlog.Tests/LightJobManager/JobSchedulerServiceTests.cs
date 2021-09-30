using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Threading.Tasks;
using TravelBlog.LightJobManager;

namespace TravelBlog.Tests.LightJobManager
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
            await jobScheduler.Enqueue(data, context);
            CollectionAssert.AreEqual(new[] { data }, (ICollection)await context.GetJobs());
            data.SetResult(true);
            await Task.Delay(100);
            CollectionAssert.AreEqual(Array.Empty<TaskCompletionSource<bool>>(), (ICollection)await context.GetJobs());
            await jobScheduler.StopAsync(default);
        }
    }
}
