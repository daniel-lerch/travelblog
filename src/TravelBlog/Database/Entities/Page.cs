namespace TravelBlog.Database.Entities;

public class Page
{
	public Page(string name, string content)
	{
		Name = name;
		Content = content;
	}

	public string Name { get; set; }
	public string Content { get; set; }
}
