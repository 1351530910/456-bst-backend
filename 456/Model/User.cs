using System;
using System.ComponentModel.DataAnnotations;

namespace bst.Model
{
    public class User
    {
        [Key]
        public Guid Userid { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        [EmailAddress(ErrorMessage ="invalid email address")]
        public string email { get; set; }
        [Phone(ErrorMessage = "invalid phone number")]
        public string phone { get; set; }


        public virtual Lab lab { get; set; }
    }
}
