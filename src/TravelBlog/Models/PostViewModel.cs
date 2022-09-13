using Microsoft.AspNetCore.Html;
using TravelBlog.Database.Entities;

namespace TravelBlog.Models;

public class PostViewModel
{
    public PostViewModel(BlogPost post, int readCount, int wordCount, string htmlContent)
    {
        Post = post;
        ReadCount = readCount;
        WordCount = wordCount;
        HtmlContent = new HtmlString(htmlContent);
    }

    public BlogPost Post { get; }
    public int ReadCount { get; }
    public int WordCount { get; }
    public HtmlString HtmlContent { get; }
}
