using Common.DTO.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest.Base.Role
{
    [TestClass]
    public class RoleControllerTests : BaseTest
    {

        private RoleController _controller = null!;

        [TestInitialize]
        public void Init()
        {
            _controller = CreateController<RoleController>();
        }


        [TestMethod]
        public async Task GetUserRoles()
        {
            var result = await _controller.GetUserRoles();
            var dto = GetResponseDto(result);
            Assert.AreEqual(dto.StatusCode, 200);

            var list = dto.Result as List<UserRoleDto>;
            Assert.AreNotEqual(list.Count, 0);
        }

        [TestMethod]
        public async Task GetRoles()
        {
            var result = await _controller.GetRoles();
            var dto = GetResponseDto(result);
            Assert.AreEqual(dto.StatusCode, 200);

            var list = dto.Result as List<RoleDto>;
            Assert.AreNotEqual(list.Count, 0);
        }

        [TestMethod]
        public async Task GetRole()
        {
            var result = await _controller.GetRole(ROLE_1_ID);
            var dto = GetResponseDto(result);
            Assert.AreEqual(dto.StatusCode, 200);

            var roleDto = dto.Result as RoleDto;
            Assert.AreEqual(roleDto.Id, ROLE_1_ID);
        }

        [TestMethod]
        public async Task UpdateRoles()
        {
            // Placeholder test for RoleController.UpdateRoles
            Assert.IsTrue(true);
            await Task.CompletedTask;
        }
    }
}
