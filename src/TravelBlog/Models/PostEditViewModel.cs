using TravelBlog.Database.Entities;

namespace TravelBlog.Models
{
    public class PostEditViewModel
    {
        public PostEditViewModel(BlogPost post)
        {
            Post = post;
        }

        public BlogPost Post { get; set; }
    }
}
