using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTest.Setup
{
    public class BaseControllerTest : BaseTest
    {
        //Generic controller creation
        public T CreateController<T>() where T : ControllerBase
        {
            var session = CreateSessionEnt(ORG_NR, USER_ACCOUNT_ID_1);

            var services = new ServiceCollection();
            services.AddSingleton(memoryCache);
            services.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory, Microsoft.Extensions.Logging.LoggerFactory>();
            services.AddSingleton(GetAuditService());
            services.AddSingleton(GetConfigService());
            services.AddSingleton(GetLabelService());
            services.AddSingleton(GetPermissionService());
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

    }
}
