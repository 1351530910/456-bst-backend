#pragma warning disable CS1701 // Assuming assembly reference matches identity

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using bst.Model;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace bst.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private UserDB context = new UserDB();

        [HttpGet,Route("")]
        public object Index()
        {
            return "success";
        }

        [HttpPost,Route("login")]
        [ProducesResponseType(typeof(LoginOut),200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<object> Login([FromBody]LoginIn data)
        {
            var user = await context.Users.Where(x => x.Email == data.Email && x.Password == data.Password).FirstOrDefaultAsync();
            if (user==null)
            {
                HttpContext.Response.StatusCode = 401;
                return "login failed";
            }
            var session = new Session
            {
                sessionid = Guid.NewGuid(),
                deviceid = data.Deviceid
            };
            AuthFilter.sessions.Add(session, user.Id);
            await context.SaveChangesAsync();
            return new LoginOut
            {
                Sessionid = session.sessionid
            };
        }

        [HttpPost,Route("createuser")]
        [ProducesResponseType(typeof(CreateUserOut),200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<object> CreateUser([FromBody]CreateUserIn user)
        {
            if (!ModelState.IsValid||user==null)
            {
                if (user==null)
                {
                    return BadRequest("received no package,, recheck frontend");
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }

            var u = new User
            {
                Id = Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password
            };
            
            context.Users.Add(u);
            await context.SaveChangesAsync();
            AuthFilter.AddSession(u.Id, user.Deviceid);

            return new CreateUserOut
            {
                Firstname = u.FirstName,
                Lastname = u.LastName,
                Email = u.Email
            };
        }

        [ProducesResponseType(200),AuthFilter]
        [ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost, Route("logout")]
        
        public async Task<object> Logout()
        {
            //var session = (Session)HttpContext.Items["session"];
            //var user = await context.users.FindAsync(HttpContext.Items["user"]);

            AuthFilter.sessions.Remove((Session)HttpContext.Items["session"]);
            return Ok();
        }


        [HttpPost, Route("listProjects"), ProducesResponseType(typeof(List<ProtocolPreview>), 200)]
        public async Task<object> ListProjects([FromBody]ListCount data)
        {
            var user = await context.Users.FindAsync(HttpContext.Items["user"]);
            return user.Protocols.Skip(data.Start).Take(data.Count).Select(x => new ProtocolPreview(x.Protocol,x.Privilege));
        }
        
    }
}
