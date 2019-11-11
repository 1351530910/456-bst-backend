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
        public string email { get; set; }
        public Guid Protocolid = Guid.Empty;    //current lock protocol
        public DateTime LastActive { get; set; }    //time of the last activity
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
        public const int EXPIRETIME = 30;   //time in minute to expire a session

        public static List<Session> sessions = new List<Session>();
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = (BaseController)context.HttpContext.Items["context"];
            var dbcontext = controller.context;

            //if not uploading a file then check if model is valid
            if (context.HttpContext.Request.ContentType != "application/octet-stream" && !context.ModelState.IsValid)
            {
                context.Result = new BadRequestResult();
                return;
            }

            //check presence of deviceid field and sessionid field
            if ((string)context.HttpContext.Request.Headers["deviceid"] == null || (string)context.HttpContext.Request.Headers["sessionid"] == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            //parse sessionid and deviceid
            var sessionid = Guid.Parse((string)context.HttpContext.Request.Headers["sessionid"]);
            var deviceid = (string)context.HttpContext.Request.Headers["deviceid"];

            //check if valid session
            var session = sessions.FirstOrDefault(x => x.Deviceid.Equals(deviceid) && x.Sessionid.Equals(sessionid));
            if (session == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            //check for expire
            if (session.LastActive.AddMinutes(EXPIRETIME) < System.DateTime.Now)
            {
                sessions.Remove(session);
                context.Result = new UnauthorizedResult();
            }

            //update active time and set the controllers required fields
            session.LastActive = System.DateTime.Now;
            var task = dbcontext.Users.FindAsync(session.Userid);
            task.Wait();
            var user = task.Result;
            if (user == null) context.Result = new UnauthorizedResult();
            controller.user = user;
            controller.session = session;

            base.OnActionExecuting(context);
        }

        public static Guid AddSession(Guid userid,string deviceid,string email)
        {
            var session = new Session
            {
                Sessionid = Guid.NewGuid(),
                Deviceid = deviceid,
                Userid = userid,
                LastActive = DateTime.Now,
                email = email
            };
            sessions.Add(session);
            return session.Sessionid;
        }
    }
    /// <summary>
    /// attribute to check if an user has the lock of a protocol
    /// enables protocol variable in basecontroller
    /// 
    /// ---this MUST happen after AuthFilter ---
    /// 
    /// </summary>
    public class WriteLock : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //find the protocol id
            string pid;
            if ((pid = context.HttpContext.Request.Headers["protocolid"])==null)
            {
                context.Result = new BadRequestObjectResult("protocolid in header not found");
                return;
            }

            //find the actual using user
            var controller = (BaseController)context.HttpContext.Items["context"];
            var protocolid = Guid.Parse(pid);

            //check if user indeed have access to protocol
            var protocoluser = controller.user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(protocolid));
            if (protocoluser==null)
            {
                context.Result = new UnauthorizedObjectResult("protocol participation not found " + controller.user.ProtocolUsers.Count);
            }

            //set controller's protocol
            controller.protocol = protocoluser.Protocol;

            //if already locked the protocol then nothing else have to be set
            if (controller.session.Protocolid == protocolid)
            {
                base.OnActionExecuting(context);
                return;
            }

            //otherwise check if locked by someone else
            Session s;
            if ((s = AuthFilter.sessions.FirstOrDefault(x => x.Protocolid.Equals(protocolid))) != null)
            {
                context.Result = new UnauthorizedObjectResult("locked by " + s.email);
            }

            //if not locked then lock the protocol
            var sessionid = Guid.Parse((string)context.HttpContext.Request.Headers["sessionid"]);
            var deviceid = (string)context.HttpContext.Request.Headers["deviceid"];
            s = AuthFilter.sessions.FirstOrDefault(x => x.Deviceid.Equals(deviceid) && x.Sessionid.Equals(sessionid));
            s.Protocolid = protocoluser.Protocol.Id;

            base.OnActionExecuting(context);
        }
    }
    /// <summary>
    /// attribute to check if an user has the lock of a protocol
    /// enables protocol variable in basecontroller
    /// 
    /// ---this MUST happen after AuthFilter ---
    /// 
    /// </summary>
    public class ReadLock : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //find the protocol id
            string pid;
            if ((pid = context.HttpContext.Request.Headers["protocolid"]) == null)
            {
                context.Result = new BadRequestObjectResult("protocolid in header not found");
                return;
            }

            //find the actual using user
            var controller = (BaseController)context.HttpContext.Items["context"];
            var protocolid = Guid.Parse(pid);

            //check if user indeed have access to protocol
            var protocoluser = controller.user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(protocolid));
            if (protocoluser == null)
            {
                context.Result = new UnauthorizedObjectResult("protocol participation not found " + controller.user.ProtocolUsers.Count);
            }

            //set controller's protocol
            controller.protocol = protocoluser.Protocol;

            //if already locked the protocol then nothing else have to be set
            if (controller.session.Protocolid == protocolid)
            {
                base.OnActionExecuting(context);
                return;
            }

            //otherwise check if locked by someone else
            Session s;
            if ((s = AuthFilter.sessions.FirstOrDefault(x => x.Protocolid.Equals(protocolid))) != null)
            {
                context.Result = new UnauthorizedObjectResult("locked by " + s.email);
            }


            base.OnActionExecuting(context);
        }
    }
}
