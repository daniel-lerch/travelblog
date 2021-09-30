using System.Collections.Generic;
using System.Threading.Tasks;

namespace TravelBlog.LightJobManager
{
    public interface IJobContext<TData>
    {
        Task Add(TData data);
        Task Remove(TData data);
        Task<IReadOnlyCollection<TData>> GetJobs();
        Task<bool> Execute(TData data);
    }
}
