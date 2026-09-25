using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GC = Backend.GlobalConstants;

namespace Backend.Core.Middleware
{
    public class VersionActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var versionAtt = context.ActionDescriptor.EndpointMetadata
                            .OfType<VersionAtt>()
                            .FirstOrDefault();

            if (versionAtt == null) return;

            foreach (var arg in context.ActionArguments.Values)
            {
                if (!Validate(arg, versionAtt))
                {
                    context.Result = new ConflictObjectResult(
                    "Version conflict detected");
                    return;
                }
            }



        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
