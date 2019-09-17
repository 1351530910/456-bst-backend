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
        public Guid Protocol = Guid.Empty;
        public DateTime LastActive { get; set; }
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
        public const int EXPIRETIME = 30;

        public static List<Session> sessions = new List<Session>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var dbcontext = (UserDB)context.HttpContext.Items["context"];
            
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestResult();
            }
            if ((string)context.HttpContext.Request.Headers["deviceid"] != null)
            {
                var sessionid = Guid.Parse((string)context.HttpContext.Request.Headers["sessionid"]);
                var deviceid = (string)context.HttpContext.Request.Headers["deviceid"];
                var session = sessions.FirstOrDefault(x => x.Deviceid.Equals(deviceid) && x.Sessionid.Equals(sessionid));
                if (session!=null)
                {
                    if (session.LastActive.AddMinutes(EXPIRETIME)<System.DateTime.Now)
                    {
                        sessions.Remove(session);
                        context.Result = new UnauthorizedResult();
                    }
                    session.LastActive = System.DateTime.Now;
                    var user = dbcontext.Users.Find(session.Userid);
                    if(user == null) context.Result = new UnauthorizedResult();
                    context.HttpContext.Items["user"] = user;
                    context.HttpContext.Items["session"] = session;

                }
                else
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }

            base.OnActionExecuting(context);
        }

        public static Guid AddSession(Guid userid,string deviceid)
        {
            var session = new Session
            {
                Sessionid = Guid.NewGuid(),
                Deviceid = deviceid,
                Userid = userid,
                LastActive = DateTime.Now
            };
            sessions.Add(session);
            return session.Sessionid;
        }

    }
}
