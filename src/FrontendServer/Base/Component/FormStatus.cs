namespace FrontendServer.Base.Component
{
    public class FormStatus
    {
        public LabelService LS { get; set; }


        public bool IsEditMode { get; set; } = false;
        
        public FormStatus ToggleEdit()
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
