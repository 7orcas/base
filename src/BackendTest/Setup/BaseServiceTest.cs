using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest.Setup
{
    public class BaseServiceTest : BaseTest
    {
        protected IServiceProvider _serviceProvider;

        //Here for convienenece
        public OrgConfigInitialiseService orgConfigInitialiseService;
        public LoginService loginservice;
        public PermissionService permissionService;
        public TokenService tokenService;
        public OrgService orgService;
        public ConfigService configService;
        public SessionService sessionService;

        public BaseServiceTest() : base()
        {
            AppSettings.DBMainConnection = ConnString;
        }

        public SessionEnt CreateSessionEnt(int OrgNr, int UserAccountId)
        {
            var session = base.CreateSessionEnt(OrgNr, UserAccountId);
            session.Org.Nr = OrgNr;
            return session;
        }

        public T CreateService<T>() where T : BaseService
        {
            _serviceProvider = BuildServiceProvider();
            return ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            // ✅ Register test audit implementation
            services.AddScoped<AuditServiceI, AuditTest>();
            services.AddSingleton<IMemoryCache>(memoryCache);

            // ✅ Register ALL services
            services.AddScoped<OrgService>();
            services.AddScoped<OrgConfigInitialiseService>();
            services.AddScoped<PermissionService>();
            services.AddScoped<TokenService>();
            services.AddScoped<ConfigService>();
            services.AddScoped<SessionService>();
            services.AddScoped<LoginService>();
            services.AddScoped<LabelService>();

            return services.BuildServiceProvider();
        }

        public class AuditTest : AuditServiceI
        {
            public Task<List<AuditList>> GetEvents(SessionEnt session) =>  throw new NotImplementedException(); 
            public void LogInOut(int sourceApp, int orgNr, int loginId, int entity) { }
            public void LogInOut(int sourceApp, long orgNr, long loginId, int entityTypeId) => throw new NotImplementedException();
            public void ReadEntity(SessionEnt session, int entityTypeId, int entityId) { }
            public void ReadEntity(SessionEnt session, int entityTypeId, long entityId) => throw new NotImplementedException();
            public void ReadList(SessionEnt session, int entityTypeId, string query) { }
        }

    }
}
