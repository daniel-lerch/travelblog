namespace TravelBlog.Models;

public class HomeEditViewModel
{
	public HomeEditViewModel(string content)
	{
		Content = content;
	}

	public string Content { get; }
}
