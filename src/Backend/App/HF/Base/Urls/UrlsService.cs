using GC = Backend.GlobalConstants;

/// <summary>
/// Urls list
/// Created: Oct 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.App.HF.Base.Urls
{
    public class UrlsService : Backend.Base.Urls.UrlsService, UrlsServiceI
    {
        
        public UrlsService(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        public string GetAppUrl() => PrefixUrl("Urls/" + GC.HF + GC.UrlList);

        public Dictionary<string, UrlDto> GetList(SessionEnt session)
        {
            var list = base.GetList(session);

            return list;
        }



    }
}
