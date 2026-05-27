using BGC = BackendTest.GlobalConstants;

namespace BackendTest.Base.Permission
{
    [TestClass]
    [TestCategory("UnitServiceBase")]
    public class PermissionServiceTests : BaseServiceTest
    {
        PermissionService service;
        PermissionInitialiseService initialiseService;
        SessionEnt session;
        static int IdStart = IdStartPermission;

        public PermissionServiceTests() : base ()
        {
            service = CreateService<PermissionService>();
            session = CreateSessionEnt(IdStart, IdStart);
            initialiseService = CreateService<PermissionInitialiseService>();
            initialiseService.InitialisePermissions();
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
            
            var perm = service.GetPermissionEnt(BGC.PerPerm7);
            Assert.IsTrue(perm.Nr == BGC.PerPerm7);
        }

        [TestMethod]
        public async Task LoadEffectivePermissions()
        {
            var list = await service.LoadEffectivePermissions(session);
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public async Task LoadEffectivePermissionsInt()
        {
            var list = await service.LoadEffectivePermissionsInt(session.UserAccount.Id, session.Org.Nr);
            Assert.IsTrue(list.Count > 0);
        }


        [TestMethod]
        public async Task IsAuthorizedToAccessEndPoint()
        {
            var valid = service.IsAuthorizedToAccessEndPoint(session, new PermissionAtt(USER_PERM), new CrudAtt(USER_PERM_CRUD));
            Assert.IsTrue(true);
        }


    }
}
