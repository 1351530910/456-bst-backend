using System;
using System.Collections.Generic;
using System.Linq;
using bst.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace bst.Controllers
{
    public class Session
    {
        public Guid sessionid { get; set; }
        public string deviceid { get; set; }
    }


    /// <summary>
    /// 
    /// sessions will keep the current active sessions
    /// if a session can be paired, the user can be found in context.items["user"]
    /// otherwise session expired
    /// 
    /// </summary>
    public class AuthFilter : ActionFilterAttribute
    {
        public static Dictionary<Session, Guid> sessions { get; set; }

        public override void OnActionExecuting(ActionExecutingContext actioncontext)
        {
            var context = new UserDB();
            
            if (!actioncontext.ModelState.IsValid)
            {
                actioncontext.Result = new BadRequestResult();
            }
            if ((string)actioncontext.HttpContext.Request.Headers["deviceid"] != null)
            {
                var sessionid = Guid.Parse((string)actioncontext.HttpContext.Request.Headers["sessionid"]);
                var deviceid = (string)actioncontext.HttpContext.Request.Headers["deviceid"];
                var session = sessions.Where(x => x.Key.deviceid.Equals(deviceid) && x.Key.sessionid.Equals(sessionid)).FirstOrDefault();
                if (session.Key!=null)
                {
                    actioncontext.HttpContext.Items["user"] = context.users.Find(session.Value);
                    actioncontext.HttpContext.Items["session"] = context.users.Find(session.Key);
                }
                else
                {
                    actioncontext.Result = new UnauthorizedResult();
                }
            }
            else
            {
                actioncontext.Result = new BadRequestResult();
            }

            base.OnActionExecuting(actioncontext);
        }

        public static Guid AddSession(Guid userid,string deviceid)
        {
            var session = new Session
            {
                sessionid = Guid.NewGuid(),
                deviceid = deviceid
            };
            sessions.Add(session, userid);
            return session.sessionid;
        }
    }
}
