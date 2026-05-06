using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTest
{
    public partial class BaseTest
    {
        public OrgConfigInitialiseService orgConfigInitialiseService;
        public LoginService loginservice;
        public PermissionService permissionService;
        public TokenService tokenService;
        public OrgService orgService;
        public ConfigService configService;
        public SessionService sessionService;

        private BaseService[] services;

        public void InitialiseServices()
        {
            // Initialize fields (assuming constructor injection or manual setup)
            orgService = new OrgService(null, memoryCache);
            orgConfigInitialiseService = new OrgConfigInitialiseService(null, memoryCache);
            permissionService = new PermissionService(null, memoryCache);
            tokenService = new TokenService(null, memoryCache);
            configService = new ConfigService(null, memoryCache);
            sessionService = new SessionService(null, memoryCache);
            loginservice = new LoginService(null, tokenService, orgService, configService, permissionService, sessionService);

            // Now safely initialize the array
            services = new BaseService[]
            {
                orgConfigInitialiseService,
                loginservice,
                permissionService,
                tokenService,
                orgService,
                configService,
                sessionService
            };

            var audit = new AuditTest();
            foreach (var service in services) 
                service._auditService = audit;
            

        }
    }

    public class AuditTest : AuditServiceI
    {
        public Task<List<AuditList>> GetEvents(SessionEnt session) { throw new NotImplementedException(); }
        public void LogInOut(int sourceApp, int orgNr, int loginId, int entity) { }

        public void LogInOut(int sourceApp, long orgNr, long loginId, int entityTypeId)
        {
            throw new NotImplementedException();
        }

        public void ReadEntity(SessionEnt session, int entityTypeId, int entityId) { }

        public void ReadEntity(SessionEnt session, int entityTypeId, long entityId)
        {
            throw new NotImplementedException();
        }

        public void ReadList(SessionEnt session, int entityTypeId, string query) { }
    }



}
