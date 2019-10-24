using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBlog.Database.Entities
{
    public class Subscriber
    {
        public Subscriber(int id, string mailAddress, string givenName, string familyName, DateTime confirmationTime, string token)
        {
            Id = id;
            MailAddress = mailAddress;
            GivenName = givenName;
            FamilyName = familyName;
            ConfirmationTime = confirmationTime;
            Token = token;
        }

        public int Id { get; set; }
        public string MailAddress { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public DateTime ConfirmationTime { get; set; }
        public string Token { get; set; }

        public IEnumerable<PostRead>? Reads { get; set; }
    }
}
