namespace FrontendServer.Base.Label
{
    public class ThemeService
    {
        public bool IsDarkMode { get; private set; }

        public event Action? OnThemeChanged;

        public void SetDarkMode(bool isDark)
        {
            IsDarkMode = isDark;
            OnThemeChanged?.Invoke();
        }
    }

}
