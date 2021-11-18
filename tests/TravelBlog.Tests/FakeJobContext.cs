using System.Collections.Generic;
using System.Threading.Tasks;
using TravelBlog.Services.LightJobManager;

namespace TravelBlog.Tests
{
    internal class FakeJobContext : IJobContext<TaskCompletionSource<bool>>
    {
        private static List<TaskCompletionSource<bool>> storage = new List<TaskCompletionSource<bool>>();

        public Task Add(TaskCompletionSource<bool> data)
        {
            storage.Add(data);
            return Task.CompletedTask;
        }

        public Task AddRange(IEnumerable<TaskCompletionSource<bool>> data)
        {
            storage.AddRange(data);
            return Task.CompletedTask;
        }

        public Task Remove(TaskCompletionSource<bool> data)
        {
            storage.Remove(data);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<TaskCompletionSource<bool>>> GetJobs()
        {
            var copy = new List<TaskCompletionSource<bool>>(storage);
            return Task.FromResult<IReadOnlyCollection<TaskCompletionSource<bool>>>(copy);
        }

        public Task<bool> Execute(TaskCompletionSource<bool> data)
        {
            return data.Task;
        }
    }
}
