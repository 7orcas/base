using QuestPDF.Fluent;

using GC = Backend.GlobalConstants;

/// <summary>
/// Pdf generation methods
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
/// 
namespace Backend.Base.Template
{
    public class PdfService : BaseService, PdfServiceI
    {

        public PdfService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public string GenerateInvoicePdf(TestInvoiceModel model)
        {
            var fileName = $"Invoice_{model.InvoiceNumber}.pdf";
            var filePath = Path.Combine("C:\\Temp", fileName);

            var document = new TestInvoiceDocument(model);

            document.GeneratePdf(filePath);

            return filePath;
        }



    }
}
