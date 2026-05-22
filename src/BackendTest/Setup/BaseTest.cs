using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection.Emit;
using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest.Setup
{
    public partial class BaseTest
    {
        private static bool initialisedDb = false;
        public readonly IMemoryCache memoryCache;
        

        public BaseTest() 
        {
            AppSettings.DBMainConnection = GCT.ConnString;
            memoryCache = new MemoryCache(new MemoryCacheOptions());
           // InitialiseServices();
        }
               

        //Generic controller creation
        public T CreateController<T>() where T : ControllerBase
        {
            var session = CreateSessionEnt();
            
            var services = new ServiceCollection();
            services.AddSingleton(memoryCache);
            services.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory, Microsoft.Extensions.Logging.LoggerFactory>();
            services.AddSingleton(GetLabelService());
            services.AddSingleton(new Mock<AuditServiceI>().Object);
            services.AddSingleton(GetPermissionInitialiseService());
            services.AddSingleton(GetRoleService());
            services.AddSingleton<System.IServiceProvider>(sp => sp);

            var sp = services.BuildServiceProvider();

            var controller = ActivatorUtilities.CreateInstance<T>(sp);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Items["session"] = session;
            return controller;
        }


        public _ResponseDto GetResponseDto(Object result)
        {
            var okResult = result as OkObjectResult;
            var dto = okResult.Value as _ResponseDto;
            return dto;
        }



        public static void ResetInitialisedDb() => initialisedDb = false;
        public bool IsInitialisedDb() => initialisedDb;


    }
}
