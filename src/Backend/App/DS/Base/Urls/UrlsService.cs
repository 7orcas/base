using GC = Backend.GlobalConstants;

/// <summary>
/// Urls list
/// Created: Oct 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.App.DS.Base.Urls
{
    public class UrlsService : Backend.Base.Urls.UrlsService, UrlsServiceI
    {
        
        public UrlsService(IServiceProvider serviceProvider)
           : base(serviceProvider)  { }

        public string GetAppUrl() => PrefixUrl("Urls/" + GC.DS + GC.UrlList);

        public Dictionary<string, UrlDto> GetList(SessionEnt session)
        {
            var urls = base.GetList(session);

            if (session.IsUserPermission(GC.PerMach))
            {
                Add(urls, "MachineList", "Machine/list");
            }


            return urls;
        }



    }
}
