namespace Backend.App.DS.Base.Urls

{
    public interface UrlsServiceI
    {
        List<UrlDto> GetList(SessionEnt session);
    }
}
