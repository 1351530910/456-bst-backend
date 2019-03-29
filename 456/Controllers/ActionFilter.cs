using System;
namespace bst.Controllers
{
    public class AuthFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Items["auth"] == null)
            {
                context.Result = new UnauthorizedResult();
            }
        }

    }
}
