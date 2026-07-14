using GC = Backend.GlobalConstants;

namespace Backend.Base.Template.Ent
{
    public abstract class BaseTemplate<E> 
    {

        public BaseTemplate(Dictionary<string, string> labels)
        {
            Data = new Dictionary<string, object>();
            Labels = labels;
        }

        public Dictionary<string, string> Labels { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public int? OrgNr { get; set; }
        public string LangCode { get; set; }
        public int? LangCodeVariant { get; set; }
        public bool IsHtml { get; set; }

        protected string GetLabel(string langKey, string nullDefault)
        {
            if (Labels.ContainsKey(langKey))
                return Labels[langKey];
            return nullDefault;
        }


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
