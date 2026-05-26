namespace BackendTest.Base.Config
{
    [TestClass]
    [TestCategory("UnitServiceBase")]
    public class ConfigServiceTests : BaseServiceTest
    {
        ConfigService service;
        SessionEnt session;
        static int IdStart = IdStartConfig;

        public ConfigServiceTests() : base()
        {
            service = CreateService<ConfigService>();
            session = CreateSessionEnt(IdStart, IdStart);
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
        }

        [ClassCleanup]
        public static async Task Cleanup()
        {
            if (RunCleanup)
                await DeleteAll(IdStart);
        }


        [TestMethod]
        public async Task CreateUserConfig()
        {
            var config = service.CreateUserConfig(session.UserAccount, session.Org, UserLangCode);
            Assert.AreEqual(session.Org.Nr, config.orgNr);
            Assert.AreEqual(OrgLangCode, UserLangCode);

            // ToDo: Add test for languages when implemented
            //foreach (var lang in Languages)
            //{
            //    if (config.Languages.Find(l => l.LangCode == lang) == null)
            //        Assert.Fail();
            //}
        }

    }
}
