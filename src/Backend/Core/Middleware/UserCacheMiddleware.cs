using JsonDiffPatchDotNet;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using GC = Backend.GlobalConstants;

namespace Backend.Core.Middleware
{
    public class UserCacheMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _log;

        public UserCacheMiddleware(
            RequestDelegate next)
        {
            _next = next;
            _log = Log.Logger;
        }

        public async Task InvokeAsync(
           HttpContext context,
           UserCacheServiceI _cacheService)
        {

            var session = null as SessionEnt;
            int entityTypeNr = -1;
            string crudAction = null;

            try
            {
                var endpoint = context.GetEndpoint();
                var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

                if (actionDescriptor != null)
                {
                    session = context.Items["session"] as SessionEnt;

                    //Method ignore
                    MethodInfo methodInfo = actionDescriptor.MethodInfo;
                    var cache = methodInfo.GetCustomAttribute<UserCacheAtt>();

                    if (cache != null)
                    {
                        //cache.CacheKey

                        var searchParameter = actionDescriptor.Parameters
                                        .OfType<ControllerParameterDescriptor>()
                                        .FirstOrDefault(p => typeof(_BaseSearch).IsAssignableFrom(p.ParameterType));


                        if (searchParameter != null)
                        {
                            context.Request.EnableBuffering();

                            using var reader = new StreamReader(
                                context.Request.Body,
                                leaveOpen: true);

                            var json = await reader.ReadToEndAsync();

                            context.Request.Body.Position = 0;
                            var searchObject = JsonSerializer.Deserialize(json, searchParameter.ParameterType);
                            var auditJson = JsonSerializer.Serialize(
                                searchObject,
                                new JsonSerializerOptions
                                {
                                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                                });

                            var delete = false;
                            if (searchObject is _BaseSearch baseSearch)
                                delete = !baseSearch.RememberSearch;
                            

                            if (delete)
                                await _cacheService.DeleteCache(session, cache.CacheKey);
                            else
                                await _cacheService.SaveCache(session, cache.CacheKey, auditJson);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error CacheMiddleware");
            }

            //Continue
            await _next(context);
        }



    }
}
