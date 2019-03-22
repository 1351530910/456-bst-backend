using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bst.Model
{
    public class CreateUserIn
    {
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string password { get; set; }
    }
    public class CreateUserOut
    {
        public Guid sessionid { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }

}
