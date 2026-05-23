// C#
using Common.DTO.Base;

namespace BackendTest.Base.Label
{

    [TestClass]
    [TestCategory("UnitControllerBase")]

    public class LabelControllerTests : BaseTest
    {
        private LabelController _controller = null!;

        [TestInitialize]
        public void Init()
        {
            _controller = CreateController<LabelController>();
        }

        [TestMethod]
        public async Task GetClientLabelList()
        {
            var result = await _controller.GetClientLabelList("en", 0);
            var dto = GetResponseDto(result);
            Assert.AreEqual(dto.StatusCode, 200);

            var list = dto.Result as List<LangLabelDto>;
            Assert.AreNotEqual(list.Count, 0);
        }

        [TestMethod]
        public async Task GetRelatedLabels()
        {
            var result = await _controller.GetRelatedLabels("Lang");
            var dto = GetResponseDto(result);
            Assert.AreEqual(dto.StatusCode, 200);

            var list = dto.Result as List<LangLabelDto>;
            Assert.AreNotEqual(list.Count, 0);
        }

    }
}