using System.Collections.Generic;
using TravelBlog.Database.Entities;

namespace TravelBlog.Models;

public class AdminViewModel
{
    public AdminViewModel(IReadOnlyList<Subscriber> pendingSubscribers, IReadOnlyList<Subscriber> confirmedSubscribers, string status)
    {
        PendingSubscribers = pendingSubscribers;
        ConfirmedSubscribers = confirmedSubscribers;
        Status = status;
    }

    public IReadOnlyList<Subscriber> PendingSubscribers { get; }
    public IReadOnlyList<Subscriber> ConfirmedSubscribers { get; }
    public string Status { get; }
}
