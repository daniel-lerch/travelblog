using Microsoft.AspNetCore.Html;

namespace TravelBlog.Models;

public class HomeViewModel
{
	public HomeViewModel(string? htmlContent)
	{
		HtmlContent = new HtmlString(htmlContent);
	}

    public HtmlString HtmlContent { get; }
}
