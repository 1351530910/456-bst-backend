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
    public class UserController : Controller
    {
        private UserDB context = new UserDB();

        [HttpGet,Route("")]
        public object Index()
        {
            return "success";
        }


        [HttpGet,Route("listuser")]
        public async Task<List<User>> List()
        {
            return await context.users.ToListAsync();
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
            user.sessionid = Guid.NewGuid();
            user.deviceid = data.deviceid;
            context.Entry(user).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return new LoginOut
            {
                sessionid = user.sessionid
            };
            
        }


        [HttpPost,Route("createuser")]
        [ProducesResponseType(typeof(CreateUserOut),200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<object> Create([FromBody]CreateUserIn user)
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
                password = user.password,
                sessionid = Guid.NewGuid(),
                deviceid = user.deviceid
            };
            
            context.users.Add(u);
            await context.SaveChangesAsync();
            return new CreateUserOut
            {
                sessionid = u.sessionid,
                firstname = u.FirstName,
                lastname = u.LastName,
                email = u.email
            };
        }
    }
}
