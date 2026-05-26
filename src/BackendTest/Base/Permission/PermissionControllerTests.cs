namespace BackendTest.Base.Permission
{
    [TestClass]
    [TestCategory("UnitControllerBase")]
    public class PermissionControllerTests : BaseControllerTest
    {
        private PermissionController _controller = null!;

        [TestInitialize]
        public void Init()
        {
            _controller = CreateController<PermissionController>();
        }

        [TestMethod]
        public async Task GetUserPermissions()
        {
            var result = await _controller.GetUserPermissions();
            var dto = GetResponseDto(result);
            Assert.AreEqual(dto.StatusCode, 200);

            var list = dto.Result as List<UserRolePermissionDto>;
            Assert.AreNotEqual(list.Count, 0);
        }

        [TestMethod]
        public async Task GetEffective()
        {
            var result = await _controller.GetEffective();
            var dto = GetResponseDto(result);
            Assert.AreEqual(dto.StatusCode, 200);

            var list = dto.Result as List<PermissionDto>;
            Assert.AreNotEqual(list.Count, 0);
        }
        
        [TestMethod]
        public async Task GetPermissionList()
        {
            var result = await _controller.GetPermissionList();
            var dto = GetResponseDto(result);
            Assert.AreEqual(dto.StatusCode, 200);

            var list = dto.Result as List<PermissionDto>;
            Assert.AreNotEqual(list.Count, 0);
        }


    }
}
