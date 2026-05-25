using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTest.Base.Label
{
    [TestClass]
    [TestCategory("UnitServiceBase")]
    public class LabelServiceTests : BaseServiceTest
    {
        LabelService service;
        SessionEnt session;

        public LabelServiceTests() : base()
        {
            service = CreateService<LabelService>();
            session = CreateSessionEnt();
        }

        [ClassInitialize]
        public static async Task InitialiseDb(TestContext context)
        {
            await SetupTestDb();
        }

        [TestMethod]
        public async Task GetLanguageLabelDic()
        {
            var dic = await service.GetLanguageLabelDic(session);
            Assert.AreNotSame(0, dic.Count);
        }

        [TestMethod]
        public async Task GetLangCodeDic()
        {
            var dic = await service.GetLangCodeDic(session);
            Assert.AreNotSame(0, dic.Count);
        }

        [TestMethod]
        public async Task GetLanguageLabel()
        {
            var dic = await service.GetLanguageLabelDic(session);
            var label = dic.Values.FirstOrDefault();

            var l = await service.GetLanguageLabel((int)label.Id);
            Assert.AreNotSame(label.Code, l.Code);
        }

        [TestMethod]
        public async Task GetRelatedLabels()
        {
            var dic = await service.GetLanguageLabelDic(session);
            var label = dic.Values.FirstOrDefault();

            var list = await service.GetRelatedLabels(label.LangKeyCode, new List<string> { label.LangCode });
            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public async Task GetAllLanguageLabels()
        {
            var list = await service.GetAllLanguageLabels();
            Assert.AreNotSame(0, list.Count);
        }

        [TestMethod]
        public async Task GetLanguageKey()
        {
            var dic = await service.GetLanguageLabelDic(session);
            var label = dic.Values.FirstOrDefault();

            var langKey = await service.GetLanguageKey(label.LangKeyCode);
            Assert.AreEqual(label.LangKeyCode, langKey.Code);
        }


    }
}

