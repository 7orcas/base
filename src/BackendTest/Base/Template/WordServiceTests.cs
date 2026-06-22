using System.IO;

namespace BackendTest.Base.Template
{
    [TestClass]
    [TestCategory("UnitServiceBase")]
    public class WordServiceTests : BaseServiceTest
    {
        WordService service;

        public WordServiceTests() : base()
        {
            service = CreateService<WordService>();
        }

        
        [TestMethod]
        public async Task GenerateDocument()
        {
            service.GenerateDocument();
            Assert.IsTrue(File.Exists(@"C:\Source\templates\Output\Result.docx"));
        }


    }
}

