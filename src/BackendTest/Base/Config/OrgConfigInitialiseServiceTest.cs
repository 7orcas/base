using Microsoft.Extensions.Caching.Memory;
using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest.Base.Config
{
    [TestClass]
    [TestCategory("UnitServiceBase")]
    public class OrgConfigInitialiseServiceTest : BaseServiceTest
    {
        OrgConfigInitialiseService service;

        public OrgConfigInitialiseServiceTest() : base()
        {
            service = CreateService<OrgConfigInitialiseService>();
        }

        [ClassInitialize]
        public static async Task InitialiseDb(TestContext context)
        {
            ResetInitialisedDb();
            await SetupTestDb();
        }

        [TestMethod]
        public async Task InitialiseOrgConfigs()
        {
            await service.InitialiseOrgConfigs();

            var orgConfig = memoryCache.Get<OrgConfig>(GC.CacheKeyOrgConfigPrefix + GCT.orgNr);

            Assert.AreEqual(GCT.orgNr, orgConfig.orgNr);
            Assert.AreEqual(GCT.OrgLangCode, orgConfig.LangCodeDefault);

            foreach (var lang in GCT.Languages)
            {
                if (orgConfig.Languages.Find(l => l.LangCode == lang) == null)
                    Assert.Fail();
            }

        }


    }
}
