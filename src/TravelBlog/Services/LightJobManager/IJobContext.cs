using System.Collections.Generic;
using System.Threading.Tasks;

namespace TravelBlog.Services.LightJobManager;

public interface IJobContext<TData>
{
    Task Add(TData data);
    Task AddRange(IEnumerable<TData> data);
    Task Remove(TData data);
    Task<IReadOnlyCollection<TData>> GetJobs();
    Task<bool> Execute(TData data);
}
