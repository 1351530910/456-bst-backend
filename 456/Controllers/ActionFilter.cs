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
        public Guid Sessionid { get; set; }
        public string Deviceid { get; set; }
        public Guid Userid { get; set; }
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


        public static List<Session> sessions = new List<Session>();

        public override void OnActionExecuting(ActionExecutingContext actioncontext)
        {
            var context = (UserDB)actioncontext.HttpContext.Items["context"];
            
            if (!actioncontext.ModelState.IsValid)
            {
                actioncontext.Result = new BadRequestResult();
            }
            if ((string)actioncontext.HttpContext.Request.Headers["deviceid"] != null)
            {
                var sessionid = Guid.Parse((string)actioncontext.HttpContext.Request.Headers["sessionid"]);
                var deviceid = (string)actioncontext.HttpContext.Request.Headers["deviceid"];
                var session = sessions.FirstOrDefault(x => x.Deviceid.Equals(deviceid) && x.Sessionid.Equals(sessionid));
                if (session!=null)
                {
                    actioncontext.HttpContext.Items["user"] = context.Users.Find(session.Userid);
                    actioncontext.HttpContext.Items["session"] = session;
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
                Sessionid = Guid.NewGuid(),
                Deviceid = deviceid,
                Userid = userid
            };
            sessions.Add(session);
            return session.Sessionid;
        }
    }
}
