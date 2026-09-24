using Backend.Core.Middleware;
using Common.Search.Base;
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
        //public OrgConfigInitialiseService orgConfigInitialiseService;
        //public LoginService loginservice;
        //public PermissionService permissionService;
        //public TokenService tokenService;
        //public OrgService orgService;
        //public ConfigService configService;
        //public SessionService sessionService;

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

        protected IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            // ✅ Register test audit implementation
            services.AddScoped<AuditServiceI, AuditTest>();
            services.AddSingleton<IMemoryCache>(memoryCache);

            // ✅ Register ALL services
            //services.AddScoped<AuditService>();
            services.AddScoped<OrgService>();
            services.AddScoped<OrgConfigInitialiseService>();
            services.AddScoped<PermissionService>();
            services.AddScoped<TokenService>();
            services.AddScoped<ConfigService>();
            services.AddScoped<SessionService>();
            services.AddScoped<LoginService>();
            services.AddScoped<LabelService>();
            services.AddScoped<EntityService>();
            services.AddScoped<WordService>();
            services.AddScoped<PdfService>();

            return services.BuildServiceProvider();
        }

        public class AuditTest : AuditServiceI
        {
            public void LogAction(SessionEnt session, int entityTypeNr, long? entityId, int? entityVersion, string crudAction, string details) { }
            public void LogIn(SessionEnt session) { }
            public void LogOut(SessionEnt session) { }
            //public void LogInOut(SessionEnt session, int entityTypeNr) => throw new NotImplementedException();
            public void ReadEntity(SessionEnt session, int entityTypeNr, int entityId) { }
            public void ReadEntity(SessionEnt session, int entityTypeNr, long entityId) => throw new NotImplementedException();
            public void ReadList(SessionEnt session, int entityTypeNr, string query) { }
                       
            Task<List<AuditEnt>> AuditServiceI.GetEvents(SessionEnt session, AuditSearch search)
            {
                throw new NotImplementedException();
            }

            Task<AuditEnt> AuditServiceI.GetById(SessionEnt session, long id)
            {
                throw new NotImplementedException();
            }
                                
           
            AuditDto AuditServiceI.PopulateDto(SessionEnt session, AuditEnt e)
            {
                throw new NotImplementedException();
            }

            void AuditServiceI.ConfigureSearch(SessionEnt session, AuditSearch search)
            {
                throw new NotImplementedException();
            }

            void AuditServiceI.LogAction(SessionEnt session, int entityTypeNr, string action, AuditInfo info)
            {
                throw new NotImplementedException();
            }
        }

    }
}
