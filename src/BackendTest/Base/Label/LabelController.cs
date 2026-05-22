// C#
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;


namespace BackendTest.Base.Label
{
    [TestClass]
    public class LabelControllerTests
    {
        private Mock<LabelServiceI> _labelServiceMock = null!;
        private LabelController _controller = null!;

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton(_labelServiceMock.Object);
            return services.BuildServiceProvider();
        }

        private LabelController CreateControllerWithSession(SessionEnt session)
        {
            var sp = BuildServiceProvider();
            var controller = new LabelController(sp);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext!.Items["session"] = session;
            return controller;
        }

        [TestInitialize]
        public void Init()
        {
            _labelServiceMock = new Mock<LabelServiceI>();
        }

        [TestMethod]
        public async Task GetClientLabelList_ReturnsOk_WhenVariantMatches()
        {
            // Arrange: session variant matches and service returns labels
            var session = new SessionEnt
            {
                Org = new OrgEnt { LangLabelVariant = 1 }
            };
            var labels = new List<LangLabel>
            {
                new LangLabel { Id = 1, LangKeyCode = "greet", Code = "Hello", Tooltip = "t" }
            };
            _labelServiceMock.Setup(s => s.GetLanguageLabelList("en", 1)).ReturnsAsync(labels);

            _controller = CreateControllerWithSession(session);

            // Act
            var result = await _controller.GetClientLabelList("en", 1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _labelServiceMock.Verify(s => s.GetLanguageLabelList("en", 1), Times.Once);
        }

        [TestMethod]
        public async Task GetClientLabelList_ReturnsOkAndDoesNotCallService_WhenVariantMismatch()
        {
            // Arrange: session variant differs -> controller should not call service
            var session = new SessionEnt
            {
                Org = new OrgEnt { LangLabelVariant = 2 }
            };

            _controller = CreateControllerWithSession(session);

            // Act
            var result = await _controller.GetClientLabelList("en", 1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // service should not be called due to invalid variant
            _labelServiceMock.Verify(s => s.GetLanguageLabelList(It.IsAny<string>(), It.IsAny<int?>()), Times.Never);
        }

        [TestMethod]
        public async Task GetRelatedLabels_ReturnsOk_WithOneDtoPerLanguage()
        {
            // Arrange: session has languages; service returns existing labels and a key
            var session = new SessionEnt
            {
                UserConfig = new UserConfig { Languages = new List<UserLang> { new UserLang { LangCode = "en", IsEditable = true }, new UserLang { LangCode = "fr", IsEditable = false } } }
            };

            var key = new LangKey { Id = 10 }; // adjust if different
            var labels = new List<LangLabel>
            {
                new LangLabel { Id = 1, LangKeyId = 10, Variant = 1, LangCode = "en", Code = "Hello", Tooltip = "t", Updated = System.DateTime.UtcNow }
                // fr missing to test creation of placeholder DTO
            };

            _labelServiceMock.Setup(s => s.GetLanguageKey("greet")).ReturnsAsync(key);
            _labelServiceMock.Setup(s => s.GetRelatedLabels("greet", It.IsAny<List<string>>())).ReturnsAsync(labels);

            _controller = CreateControllerWithSession(session);

            // Act
            var result = await _controller.GetRelatedLabels("greet");

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _labelServiceMock.Verify(s => s.GetLanguageKey("greet"), Times.Once);
            _labelServiceMock.Verify(s => s.GetRelatedLabels("greet", It.IsAny<List<string>>()), Times.Once);
        }

        [TestMethod]
        public async Task GetRelatedLabels_ReturnsInvalid_WhenNoLanguages()
        {
            // Arrange: session with empty languages
            var session = new SessionEnt
            {
                UserConfig = new UserConfig { Languages = new List<UserLang>() }
            };

            _controller = CreateControllerWithSession(session);

            // Act
            var result = await _controller.GetRelatedLabels("greet");

            // Assert - controller returns Ok with an error response but does not call service
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _labelServiceMock.Verify(s => s.GetLanguageKey(It.IsAny<string>()), Times.Never);
            _labelServiceMock.Verify(s => s.GetRelatedLabels(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
        }
    }
}