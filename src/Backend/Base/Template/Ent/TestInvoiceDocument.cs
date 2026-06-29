using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Backend.Base.Template.Ent
{
    public class TestInvoiceDocument : IDocument
    {
        private readonly TestInvoiceModel _model;

        public TestInvoiceDocument(TestInvoiceModel model)
        {
            _model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(50);

                page.Header()
                    .Text($"Invoice #{_model.InvoiceNumber}")
                    .FontSize(20)
                    .Bold();

                page.Content()
                    .PaddingVertical(10)
                    .Column(col =>
                    {
                        col.Spacing(5);

                        col.Item().Text($"Customer: {_model.CustomerName}");
                        col.Item().Text($"Date: {_model.InvoiceDate:dd MMM yyyy}");
                        col.Item().Text($"Amount: ${_model.Amount}");
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span(DateTime.Now.ToString("dd MMM yyyy"))
                         .SemiBold();
                    });
            });
        }
    }
}