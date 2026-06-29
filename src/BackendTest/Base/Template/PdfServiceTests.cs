using Backend.Base.Template.Ent;
using QuestPDF.Infrastructure;
using System.IO;

namespace BackendTest.Base.Template
{
    [TestClass]
    [TestCategory("UnitServiceBase")]
    public class PdfServiceTests : BaseServiceTest
    {
        PdfService service;

        public PdfServiceTests() : base()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            service = CreateService<PdfService>();
        }


        [TestMethod]
        public async Task GenerateDocument()
        {
            var model = new TestInvoiceModel
            {
                CustomerName = "John Stewart",
                InvoiceNumber = "TEST-123",
                InvoiceDate = DateTime.Now,
                Amount = 99.99m

            };

            var path = service.GenerateInvoicePdf(model);
            Assert.IsTrue(File.Exists(path));
        }


    }
}

