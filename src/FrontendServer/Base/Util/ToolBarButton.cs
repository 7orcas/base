namespace FrontendServer.Base.Util
{
    public class ToolBarButton
    {
        public string? LangKey { get; set; }
        public string? Label { get; set; }
        public string? Class { get; set; }
        public Func<Task> Action { get; set; }
        public Func<string> GetLabel { get; set; }
        public bool IsLabel() => Label != null;
        public bool IsLabelFn() => GetLabel != null;
    }
}
