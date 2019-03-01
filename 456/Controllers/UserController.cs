#pragma warning disable CS1701 // Assuming assembly reference matches identity

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bst.Model;

namespace bst.Controllers
{
    public class UserController : Controller
    {
        private UserDB context = new UserDB();
        [HttpGet]
        public async Task<List<User>> List()
        {
            return await context.users.ToListAsync();
        }
        [HttpPost]
        public async Task<Object> Create([FromBody]User user)
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
