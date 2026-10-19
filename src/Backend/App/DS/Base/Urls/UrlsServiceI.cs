namespace Backend.App.DS.Base.Urls

{
    public interface UrlsServiceI
    {
        string GetAppUrl();
        public Dictionary<string, UrlDto> GetList(SessionEnt session);
    }
}
