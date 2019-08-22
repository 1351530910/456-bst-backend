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
        public async Task<object> login([FromBody]LoginIn data)
        {
            var user = await context.users.Where(x => x.email == data.email && x.password == data.password).FirstOrDefaultAsync();
            if (user==null)
            {
                HttpContext.Response.StatusCode = 401;
                return "login failed";
            }
            var session = new Session
            {
                sessionid = Guid.NewGuid(),
                deviceid = data.deviceid
            };
            AuthFilter.sessions.Add(session, user.id);
            await context.SaveChangesAsync();
            return new LoginOut
            {
                sessionid = session.sessionid
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
                id = Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                email = user.email,
                password = user.password
            };
            
            context.users.Add(u);
            await context.SaveChangesAsync();
            return new CreateUserOut
            {
                firstname = u.FirstName,
                lastname = u.LastName,
                email = u.email
            };
        }

        [ProducesResponseType(200),AuthFilter]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, Route("logout")]
        
        public async Task<object> logout()
        {
            try
            {
                //check for active session
                if (HttpContext.Items["user"]==null)
                {
                    return Unauthorized();
                }

                AuthFilter.sessions.Remove((Session)HttpContext.Items["session"]);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpPost, Route("listProjects"), ProducesResponseType(typeof(List<ProtocolPreview>), 200)]
        public async Task<object> listProjects([FromBody]ListCount data)
        {

            throw new NotImplementedException();
        }
        
    }
}
