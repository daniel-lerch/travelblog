namespace TravelBlog.Models;

public class StatusViewModel
{
    public StatusViewModel(int statusCode, string statusDescription)
    {
        StatusCode = statusCode;
        StatusDescription = statusDescription;
    }

    public int StatusCode { get; }
    public string StatusDescription { get; }
}
