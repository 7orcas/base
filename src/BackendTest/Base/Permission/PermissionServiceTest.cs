using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest.Base.Permission
{
    [TestClass]
    public class PermissionServiceTest : BaseTest
    {
        PermissionService service;

        public PermissionServiceTest() : base ()
        {
            service = permissionService;
        }

        //[AssemblyInitialize]
        [ClassInitialize]
        public static async Task InitialiseDb(TestContext context)
        {
            ResetInitialisedDb();
            await SetupTestDb();
        }

        [TestMethod]
        public async Task LoadPermissions()
        {
            var list = await service.LoadEffectivePermissionsInt(GCT.UserId, GCT.orgNr);
            Assert.AreEqual(MaxRoles, list.Count);
            
            bool v = true;
            foreach (var permission in list)
            {
                if (permission.Crud != "crud")
                    v = false;
            }
            if (!v) Assert.Fail();

        }


    }
}
