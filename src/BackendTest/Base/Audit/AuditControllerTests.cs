namespace BackendTest.Base.Audit
{
    [TestClass]
    [TestCategory("UnitControllerBase")]
    public class AuditControllerTests : BaseControllerTest
    {
        private AuditController _controller = null!;

        [TestInitialize]
        public void Init()
        {
            _controller = CreateController<AuditController>();
        }

        [TestMethod]
        public async Task Get()
        {
            var result = await _controller.Get();
            var dto = GetResponseDto(result);
            Assert.AreEqual(dto.StatusCode, 200);

            var list = dto.Result as List<AuditDto>;
            Assert.IsTrue(list.Count > 0);
        }

    }
}
