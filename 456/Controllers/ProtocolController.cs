using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Filters;
using bst.Model;
using Microsoft.EntityFrameworkCore;


namespace bst.Controllers
{
    [Route("protocol")]
    public class ProtocolController : Controller
    {

        public UserDB db { get; set; }
        public User u { get; set; }



        public override void OnActionExecuting(ActionExecutingContext context)
        {
            db = new UserDB();
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
            }
            
            if ((string)HttpContext.Request.Headers["deviceid"]!=null)
            {
                var sessionid = Guid.Parse((string)HttpContext.Request.Headers["sessionid"]);
                var deviceid = (string)HttpContext.Request.Headers["deviceid"];
                u = db.users.Where(x => x.deviceid == deviceid && x.sessionid.Equals(sessionid)).FirstOrDefault();
                HttpContext.Items["auth"] = 1;
            }

            base.OnActionExecuting(context);
        }

        [AuthFilter,HttpPost,Route("get/{protocolid}")]
        public async Task<object> getprotocol(Guid protocolid)
        {
            var p = await db.Protocols.FindAsync(protocolid);
            if (p.LockedUser==null)
            {
                p.LockedUser = u;
                db.Entry(p).State = EntityState.Modified;
            }
            if (p.LockedUser!=u)
            {
                return Unauthorized("resource locked by other user");
            }


        }
    }
}
