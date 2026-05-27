using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTest.Base.Audit
{
    [TestClass]
    [TestCategory("UnitServiceBase")]
    public class AuditServiceTests : BaseServiceTest
    {
        AuditService service;
        SessionEnt session;
        static int IdStart = IdStartAudit;

        public AuditServiceTests() : base()
        {
            _serviceProvider = BuildServiceProviderAudit();
            service = ActivatorUtilities.CreateInstance<AuditService>(_serviceProvider);
            session = CreateSessionEnt(IdStart, IdStart);
        }

        protected IServiceProvider BuildServiceProviderAudit()
        {
            var services = new ServiceCollection();
            services.AddScoped<AuditService>();
            services.AddScoped<EntityServiceI, EntityService>();
            return services.BuildServiceProvider();
        }


        [ClassInitialize]
        public static async Task InitialiseDb(TestContext context)
        {
            await SetupTestDb();
            await DeleteAll(IdStart);
            await InsertOrg(IdStart);
            await InsertUser(IdStart);
            await InsertRole(IdStart);
            await InsertUserAcc(IdStart);
            await InsertUserAccRole(IdStart);
            await InsertAudit(IdStart);
        }

        [ClassCleanup]
        public static async Task Cleanup()
        {
            if (RunCleanup)
                await DeleteAll(IdStart);
        }


        [TestMethod]
        public async Task GetAudits()
        {
            var list = await service.GetEvents(session);
            Assert.IsTrue(list.Count > 0);
        }
    }
}
