namespace Backend.Base.Urls
{
    public interface UrlsServiceI
    {
        List<UrlDto> GetList(SessionEnt session);
    }
}
