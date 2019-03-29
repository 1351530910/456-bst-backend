using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Filters;
using bst.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bst.Controllers
{

    public class ProtocolController : Controller
    {

        public UserDB userdb { get; set; }
        public User u { get; set; }



        public override void OnActionExecuting(ActionExecutingContext context)
        {
            userdb = new UserDB();
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
            }
            if ((string)HttpContext.Request.Headers["deviceid"]!=null)
            {
                var sessionid = Guid.Parse((string)HttpContext.Request.Headers["sessionid"]);
                var deviceid = (string)HttpContext.Request.Headers["deviceid"];
                u = userdb.users.Where(x => x.deviceid == deviceid && x.sessionid.Equals(sessionid)).FirstOrDefault();
                HttpContext.Items["auth"] = 1;
            }

            base.OnActionExecuting(context);
        }

        [AuthFilter]
        public Task<object> getprotocol()
        {
            throw new NotImplementedException();
        }






    }


}
