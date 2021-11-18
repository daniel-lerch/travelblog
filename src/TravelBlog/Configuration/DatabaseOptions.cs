using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TravelBlog.Configuration
{
    public class DatabaseOptions
    {
        [Required, NotNull] public string? ConnectionString { get; set; }
    }
}
