using Microsoft.Extensions.Caching.Memory;
using Superpower.Parsers;
using GC = Backend.GlobalConstants;

namespace BackendTest.Base.Config
{
    [TestClass]
    [TestCategory("UnitServiceBase")]
    public class OrgConfigInitialiseServiceTests : BaseServiceTest
    {
        OrgConfigInitialiseService service;
        SessionEnt session;
        static int IdStart = IdStartConfigInitial;

        public OrgConfigInitialiseServiceTests() : base()
        {
            service = CreateService<OrgConfigInitialiseService>();
            session = CreateSessionEnt(IdStart, IdStart);
        }

        [ClassInitialize]
        public static async Task InitialiseDb(TestContext context)
        {
            await SetupTestDb();
            await DeleteAll(IdStart);
            await InsertOrg(IdStart);
        }

        [ClassCleanup]
        public static async Task Cleanup()
        {
            if (RunCleanup)
                await DeleteAll(IdStart);
        }

        [TestMethod]
        public async Task InitialiseOrgConfigs()
        {
            await service.InitialiseOrgConfigs();

            var orgConfig = memoryCache.Get<OrgConfig>(GC.CacheKeyOrgConfigPrefix + session.Org.Nr);

            Assert.AreEqual(session.Org.Nr, orgConfig.orgNr);
            Assert.AreEqual(session.UserConfig.LangCodeCurrent, orgConfig.LangCodeDefault);

            //foreach (var lang in GCT.Languages)
            //{
            //    if (orgConfig.Languages.Find(l => l.LangCode == lang) == null)
            //        Assert.Fail();
            //}

        }


    }
}
