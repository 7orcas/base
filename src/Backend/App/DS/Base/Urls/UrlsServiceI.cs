namespace Backend.App.DS.Base.Urls

{
    public interface UrlsServiceI
    {
        string GetAppUrl();
        List<UrlDto> GetList(SessionEnt session);
    }
}
