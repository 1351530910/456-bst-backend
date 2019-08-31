#pragma warning disable CS1701 // Assuming assembly reference matches identity

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bst.Model;
using Microsoft.AspNetCore.Http;

namespace bst.Controllers
{
    [Route("user")]
    public class UserController : BaseController
    {
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
            var session = AuthFilter.AddSession(user.Id, data.Deviceid);
            await context.SaveChangesAsync();
            return new LoginOut
            {
                Sessionid = session
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
            var sessionid = AuthFilter.AddSession(u.Id, user.Deviceid);

            return new CreateUserOut
            {
                Sessionid = sessionid,
                Firstname = u.FirstName,
                Lastname = u.LastName,
                Email = u.Email
            };
        }

        [ProducesResponseType(200),AuthFilter]
        [ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost, Route("logout")]       
        public object Logout()
        {
            //var session = (Session)HttpContext.Items["session"];
            //var user = await context.users.FindAsync(HttpContext.Items["user"]);

            AuthFilter.sessions.Remove((Session)HttpContext.Items["session"]);
            return Ok();
        }


        [HttpPost, Route("listProjects"), ProducesResponseType(typeof(List<ProtocolData>), 200),AuthFilter]
        public List<ProtocolData> ListProjects([FromBody]ListCount data)
        {
            var user = (User)HttpContext.Items["user"];
            if (user.Protocols != null)
            {
                return user.Protocols.Skip(data.Start).Take(data.Count).Select(x => new ProtocolData(x.Protocol, x.Privilege)).ToList();
            }
            else
            {
                return new List<ProtocolData>();
            }
        }
        
    }
}
