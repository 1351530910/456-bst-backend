using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bst.Model
{
    public class CreateUserIn
    {
        [EmailAddress,Required]
        public string email { get; set; }
        [MinLength(8), MaxLength(15), Required]
        public string password { get; set; }
        [MaxLength(30),Required]
        public string FirstName { get; set; }
        [MaxLength(30),Required]
        public string LastName { get; set; }
        [Required]
        public string deviceid { get; set; }
    }
    public class CreateUserOut
    {
        public Guid sessionid { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }

    public class LoginIn
    {
        [EmailAddress,Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string deviceid { get; set; }
    }
    public class LoginOut
    {
        public Guid sessionid { get; set; }
    }

}
