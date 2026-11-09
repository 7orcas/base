namespace FrontendServer.Base.Util
{
    public class FilterField <T> where T : _BaseDto
    {
        public bool ShowFilter { get; set; } = false;
        public string Code { get; set; }  = string.Empty;

        public void ToggleFilter() => ShowFilter = !ShowFilter;

        public bool Filter(T item) 
        {
            if (string.IsNullOrWhiteSpace(Code))
                return true;

            return item.Code?.Contains(
                Code,
                StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}
