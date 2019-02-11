using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bst.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bst.Controllers
{
    [Route("api/User")]
    public class UserController : Controller
    {
        UserDB context = new UserDB();
        // GET: api/values
        [HttpGet]
        public Task<List<User>> Get()
        {
            return context.users.ToListAsync();
        }
    }
}
