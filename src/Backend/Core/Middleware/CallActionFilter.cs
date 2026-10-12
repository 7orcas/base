using Microsoft.AspNetCore.Mvc.Filters;
using GC = Backend.GlobalConstants;

namespace Backend.Core.Middleware
{
    public class CallActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //Get request arguments
            var http = context.HttpContext;
            http.Items[GC.RestActionArg] = new Dictionary<string, object?>(context.ActionArguments);
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
