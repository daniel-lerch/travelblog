using TravelBlog.Database.Entities;

namespace TravelBlog.Models;

public class UnsubscribeViewModel
{
    public UnsubscribeViewModel(Subscriber subscriber)
    {
        Subscriber = subscriber;
    }

    public Subscriber Subscriber { get; }
}
