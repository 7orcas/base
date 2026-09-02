namespace FrontendServer.Base.Component
{
    public class PageStatus
    {
        public LabelService LS { get; set; }
        public bool isCrudC = false;
        public bool isCrudR = false;
        public bool isCrudU = false;
        public bool isCrudD = false;
        public bool isCrudRO = true;
        public string? pageCode;


        public bool orCrudCUD()
        {
            return isCrudC || isCrudU || isCrudD;
        }


        public bool IsEditMode { get; set; } = false;
        
        public PageStatus ToggleEdit()
        {
            IsEditMode = !IsEditMode;
            return this;
        }

        public string GetEditModeLabel()
        {
            if (LS == null) return IsEditMode ? "ViewX" : "EditX";
            return IsEditMode ? LS.GetLabel("ViewM") : LS.GetLabel("EditM");
        }
    }
}
