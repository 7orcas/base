namespace BackendTest.Base.Permission
{
    [TestClass]
    [DoNotParallelize]
    [TestCategory("UnitServiceBase")]
    public class PermissionServiceTests : BaseServiceTest
    {
        PermissionService service;
        SessionEnt session;
        static int IdStart = IdStartPermission;

        public PermissionServiceTests() : base ()
        {
            service = CreateService<PermissionService>();
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
            await InsertRolePermission(IdStart);
        }

        [ClassCleanup]
        public static async Task Cleanup()
        {
            if (RunCleanup)
                await DeleteAll(IdStart);
        }


        [TestMethod]
        public async Task GetPermissions()
        {
            var list = await service.GetPermissions(session);
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public async Task GetPermissionEnt()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task LoadEffectivePermissions()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task LoadEffectivePermissionsInt()
        {
            Assert.IsTrue(true);
        }


        [TestMethod]
        public async Task IsAuthorizedToAccessEndPoint()
        {
            Assert.IsTrue(true);
        }


    }
}
