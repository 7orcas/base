
using GC = Backend.GlobalConstants;

/// <summary>
/// Template generation methods
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
/// 
namespace Backend.Base.Template
{
    public class TemplateService : BaseService, TemplateServiceI
    {
        public TemplateService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        //public byte[] GenerateDocument(string templatePath)
        //{
        //    using var doc = DocX.Load(templatePath);

        //    doc.ReplaceText("{CustomerName}", "Acme Ltd");
        //    doc.ReplaceText("{ContactFirstName}", "John");
        //    doc.ReplaceText("{ReportDate}", DateTime.Now.ToShortDateString());

        //    using var ms = new MemoryStream();
        //    doc.SaveAs(ms);

        //    return ms.ToArray();
        //}


    }
}
