using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.Core.Middleware
{
    public class AuditActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["ActionArguments"] =
                new Dictionary<string, object?>(context.ActionArguments);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
