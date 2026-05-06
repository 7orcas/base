using Backend.Base.Entity;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest.Base.Config
{
    [TestClass]
    public class ConfigServiceTest : BaseTest
    {
        ConfigService service;
        OrgEnt org;
        OrgConfig orgConfig;
        //UserEnt user;
        UserConfig userConfig;

        public ConfigServiceTest() : base()
        {
            service = new ConfigService(null, memoryCache);
        }

        [ClassInitialize]
        public static async Task InitialiseDb(TestContext context)
        {
            ResetInitialisedDb();
            await SetupTestDb();
        }

        private async Task Setup()
        {
            org = await orgService.GetOrg(GCT.OrgNr);
            await orgConfigInitialiseService.InitialiseOrgConfigs();
            orgConfig = memoryCache.Get<OrgConfig>(GC.CacheKeyOrgConfigPrefix + GCT.orgNr);
        }

        [TestMethod]
        public async Task ConfigOrgs()
        {
            await Setup();
            Assert.AreEqual(GCT.orgNr, orgConfig.orgNr);
            Assert.AreEqual(GCT.OrgLangCode, orgConfig.LangCodeDefault);
            foreach (var lang in GCT.Languages)
            {
                if (orgConfig.Languages.Find(l => l.LangCode == lang) == null)
                    Assert.Fail();
            }
        }

        //[TestMethod]
        //public async Task ConfigUser()
        //{
        //    await Setup();
        //    user = await GetUserEnt();
        //    userConfig = configService.CreateUserConfig(user, org, GCT.UserLangCode);

        //    Assert.AreEqual(GCT.orgNr, userConfig.orgNr);
        //    Assert.AreEqual(GCT.UserLangCode, userConfig.LangCodeCurrent);
        //    foreach (var lang in GCT.Languages)
        //    {
        //        if (userConfig.Languages.Find(l => l.LangCode == lang) == null)
        //            Assert.Fail();
        //    }
        //}
    }
}
