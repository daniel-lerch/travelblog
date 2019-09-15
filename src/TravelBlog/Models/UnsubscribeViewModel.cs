using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBlog.Database.Entities;

namespace TravelBlog.Models
{
    public class UnsubscribeViewModel
    {
        public UnsubscribeViewModel(Subscriber subscriber)
        {
            Subscriber = subscriber;
        }

        public Subscriber Subscriber { get; }
    }
}
