namespace BackendTest.Base.Permission
{
    [TestClass]
    [TestCategory("UnitServiceBase")]

    public class PermissionInitialiseServiceTests : BaseServiceTest
    {
        PermissionInitialiseService service;
        public PermissionInitialiseServiceTests()
        {
            service = CreateService<PermissionInitialiseService>();
            service.InitialisePermissions();
        }

        [TestMethod]
        public async Task GetPermissions()
        {
            var dic = service.GetPermissions();
            Assert.IsTrue(dic.Count > 0);
        }

    }
}
