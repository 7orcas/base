using JsonDiffPatchDotNet;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using GC = Backend.GlobalConstants;

namespace Backend.Core.Middleware
{
    /**
     * Version check
     */
    public class VersionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _log;

        public VersionMiddleware(
            RequestDelegate next)
        {
            _next = next;
            _log = Log.Logger;
        }

        public async Task InvokeAsync(
           HttpContext context,
           VersionRepoI _versionRepo)
        {

            try
            {
                var endpoint = context.GetEndpoint();
                var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

                if (actionDescriptor != null)
                {
                    //Method ignore
                    MethodInfo methodInfo = actionDescriptor.MethodInfo;
                    var version = methodInfo.GetCustomAttribute<VersionAtt>();

                    if (version != null)
                    {
                        if (context.Items.TryGetValue(GC.RestActionArg, out var value) 
                            && value is Dictionary<string, object?> args)
                        {
                            foreach (var arg in args.Values)
                            {
                                ProcessUpdateRequest(arg);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error VersionMiddleware");
            }

            //Continue
            await _next(context);
        }

        private void ProcessUpdateRequest(object? arg)
        {
            if (arg == null)
                return;

            var type = arg.GetType();

            // Is it UpdateRequest<T>?
            if (!type.IsGenericType ||
                type.GetGenericTypeDefinition() != typeof(UpdateRequest<>))
                return;

            var updatesProp = type.GetProperty("Updates");
            var updates = updatesProp?.GetValue(arg) as System.Collections.IEnumerable;

            if (updates == null)
                return;

            foreach (var item in updates)
            {
                ProcessDto(item);
            }
        }

        private void ProcessDto(object dto)
        {
            var dtoType = dto.GetType();

            var nr = dtoType.GetProperty("Nr")?.GetValue(dto);
            var version = dtoType.GetProperty("Version")?.GetValue(dto);

            Console.WriteLine($"Nr={nr}, Version={version}");
        }

    }
}
