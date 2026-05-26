namespace BackendTest.Base.Config
{
    [TestClass]
    [TestCategory("UnitControllerBase")]
    public class ConfigControllerTests : BaseControllerTest
    {
        private ConfigController _controller = null!;

        [TestInitialize]
        public void Init()
        {
            _controller = CreateController<ConfigController>();
        }
    
        [TestMethod]
        public async Task GetClientConfig()
        {
            var result = await _controller.GetClientConfig();
            var dto = GetResponseDto(result);
            Assert.AreEqual(dto.StatusCode, 200);

            var config = dto.Result as AppConfigDto;
            Assert.AreEqual(config.OrgDescription, ORG_DESC);
        }
    }
}
