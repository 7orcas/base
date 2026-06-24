using GC = Backend.GlobalConstants;

namespace Backend.Base.Template.Ent
{
    public abstract class BaseTemplate<E> 
    {

        public BaseTemplate(GC.TemplateType templateType)
        {
            TemplateType = templateType;
            Data = new Dictionary<string, object>();
        }

        public GC.TemplateType TemplateType { get; }

        public Dictionary<string, object> Data { get; set; }
        public string LangCode { get; set; }
        public bool IsHtml { get; set; }

        protected abstract string Template();

        public string RenderTemplate()
        {
            var t = Template();

            if (string.IsNullOrWhiteSpace(t))
                throw new ArgumentException("Template cannot be empty");

            var template = Scriban.Template.Parse(t);

            if (template.HasErrors)
                throw new Exception("Template parsing failed");

            var result = template.Render(Data, member => member.Name);

            return result;
        }

    }
}
