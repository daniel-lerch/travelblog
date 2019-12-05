using System;
using System.Collections.Generic;
using System.Linq;

namespace TravelBlog.Database.Entities
{
    public class Subscriber
    {
        public Subscriber(string? mailAddress, string givenName, string familyName, string? token) 
            : this(default, mailAddress, givenName, familyName, default, default, token) { }

        public Subscriber(int id, string? mailAddress, string givenName, string familyName, DateTime confirmationTime, DateTime deletionTime, string? token)
        {
            Id = id;
            MailAddress = mailAddress;
            GivenName = givenName;
            FamilyName = familyName;
            ConfirmationTime = confirmationTime;
            DeletionTime = deletionTime;
            Token = token;
        }

        public int Id { get; set; }
        public string? MailAddress { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public DateTime ConfirmationTime { get; set; }
        public DateTime DeletionTime { get; set; }
        public string? Token { get; set; }

        public IEnumerable<PostRead>? Reads { get; set; }
    }
}
