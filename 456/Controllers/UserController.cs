#pragma warning disable CS1701 // Assuming assembly reference matches identity

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using bst.Model;

namespace bst.Controllers
{
    public class Session
    {
        public Guid guid { get; set; }
        public Guid userid { get; set; }
        public Timer timer { get; set; }
    }
    public class UserController : Controller
    {
        private UserDB context = new UserDB();
        private List<Session> sessions = new List<Session>();
        [HttpGet,Route("listuser")]
        public async Task<List<User>> List()
        {
            return await context.users.ToListAsync();
        }
        [HttpPost,Route("createuser")]
        public async Task<object> Create([FromBody]User user)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.Values.SelectMany(v=>v.Errors);
            }
            user.id = new Guid();
            context.users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }
    }
}
