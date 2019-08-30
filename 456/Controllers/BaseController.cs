using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bst.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace bst.Controllers
{
    public class BaseController : Controller
    {
        public UserDB context;

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            this.context = new UserDB();
            HttpContext.Items["context"] = this.context;

            base.OnActionExecuting(context);
            
        }
    }
}