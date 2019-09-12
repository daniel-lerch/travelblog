using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBlog.Database.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string MailAddress { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public DateTime ConfirmationTime { get; set; }
    }
}
