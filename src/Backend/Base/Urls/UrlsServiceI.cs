namespace Backend.Base.Urls
{
    public interface UrlsServiceI
    {
        string GetAppUrl();
        Dictionary<string, UrlDto> GetList(SessionEnt session);
    }
}
