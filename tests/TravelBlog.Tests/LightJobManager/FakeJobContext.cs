using System.Collections.Generic;
using System.Threading.Tasks;
using TravelBlog.LightJobManager;

namespace TravelBlog.Tests.LightJobManager
{
    internal class FakeJobContext : IJobContext<TaskCompletionSource<bool>>
    {
        private static List<TaskCompletionSource<bool>> storage = new List<TaskCompletionSource<bool>>();

        public Task Add(TaskCompletionSource<bool> data)
        {
            storage.Add(data);
            return Task.CompletedTask;
        }

        public Task Remove(TaskCompletionSource<bool> data)
        {
            storage.Remove(data);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<TaskCompletionSource<bool>>> GetJobs()
        {
            return Task.FromResult<IReadOnlyCollection<TaskCompletionSource<bool>>>(storage);
        }

        public Task<bool> Execute(TaskCompletionSource<bool> data)
        {
            return data.Task;
        }
    }
}
