using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bst.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace bst.Controllers
{
    public struct Session
    {
        public Guid sessionid { get; set; }
        public string deviceid { get; set; }
    }
    [ApiController]
    public class AuthController : Controller
    {
        public UserDB context { get; set; }
        public User u { get; set; }

        public static Dictionary<Session,Guid> sessions { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this.context = new UserDB();
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
            }

            if ((string)HttpContext.Request.Headers["deviceid"] != null)
            {
                var sessionid = Guid.Parse((string)HttpContext.Request.Headers["sessionid"]);
                var deviceid = (string)HttpContext.Request.Headers["deviceid"];
                var session = sessions.Where(x => x.Key.deviceid.Equals(deviceid) && x.Key.sessionid.Equals(sessionid)).FirstOrDefault();
                u = this.context.users.Find(session.Value);
                if (u!=null)
                {
                    HttpContext.Items["auth"] = 1;
                }
            }

            base.OnActionExecuting(context);
        }




    }
}