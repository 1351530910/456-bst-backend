using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace bst.Controllers
{
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Items["auth"] == null)
            {
                context.Result = new UnauthorizedResult();
            }
            base.OnActionExecuting(context);
        }

    }
}
