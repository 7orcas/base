using System.Reflection.Emit;
using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest.Base.Permission
{
    [TestClass]
    [DoNotParallelize]
    [TestCategory("UnitServiceBase")]
    public class PermissionServiceTest : BaseServiceTest
    {
        PermissionService service;
        SessionEnt session;

        public PermissionServiceTest() : base ()
        {
            service = CreateService<PermissionService>();
            session = CreateSessionEnt();
        }

        [ClassInitialize]
        public static async Task InitialiseDb(TestContext context)
        {
            await SetupTestDb();
            await DeleteAll();
            await InsertOrg();
            await InsertUser();
            await InsertRole();
            await InsertUserAcc();
            await InsertUserAccRole();
            await InsertRolePermission();
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
