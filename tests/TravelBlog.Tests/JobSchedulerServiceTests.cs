using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using TravelBlog.Services.LightJobManager;
using Xunit;

namespace TravelBlog.Tests;

public class JobSchedulerServiceTests
{
    [Fact]
    public async Task TestStart()
    {
        var context = new FakeJobContext();
        var data = new TaskCompletionSource<bool>();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var jobScheduler = new JobSchedulerService<TaskCompletionSource<bool>, FakeJobContext>(serviceProvider);

        await jobScheduler.StartAsync(default);
        await jobScheduler.Enqueue(data);
        Assert.Equal(new[] { data }, await context.GetJobs());
        data.SetResult(true);
        await Task.Delay(50);
        Assert.Empty(await context.GetJobs());
        await jobScheduler.StopAsync(default);
    }

    [Fact]
    public async Task TestStop()
    {
        var context = new FakeJobContext();
        var data = new TaskCompletionSource<bool>();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var jobScheduler = new JobSchedulerService<TaskCompletionSource<bool>, FakeJobContext>(serviceProvider);

        await jobScheduler.StartAsync(default);
        await jobScheduler.Enqueue(data);
        Assert.Equal(new[] { data }, await context.GetJobs());
        await Task.Delay(50);
        Task stop = jobScheduler.StopAsync(default);
        await Task.Delay(50);
        Assert.False(stop.IsCompleted);
        data.SetResult(true);
        await stop;
        Assert.Empty(await context.GetJobs());
    }

    [Fact]
    public async Task TestStartMultiple()
    {
        var context = new FakeJobContext();
        var data = new[] { new TaskCompletionSource<bool>(), new TaskCompletionSource<bool>(), new TaskCompletionSource<bool>() };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var jobScheduler = new JobSchedulerService<TaskCompletionSource<bool>, FakeJobContext>(serviceProvider);

        await jobScheduler.StartAsync(default);
        await jobScheduler.Enqueue(data);
        Assert.Equal(data, await context.GetJobs());
        for (int i = 0; i < data.Length; i++)
        {
            data[i].SetResult(true);
            await Task.Delay(50);
            Assert.Equal(data.Length - i - 1, (await context.GetJobs()).Count);
        }
        await jobScheduler.StopAsync(default);
    }
}
