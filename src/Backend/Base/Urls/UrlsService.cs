using GC = Backend.GlobalConstants;

/// <summary>
/// Urls list
/// Created: Oct 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Urls
{
    public class UrlsService : BaseService, UrlsServiceI
    {
        
        public UrlsService(IServiceProvider serviceProvider)
           : base(serviceProvider)  { }

        private const string UrlBase = "api/";
        private readonly string URL_user = UrlBase + "User/";
        private readonly string UrlOrg = UrlBase + "Org/";
        private readonly string URL_audit = UrlBase + "Audit/";

        public string GetAppUrl() => PrefixUrl("Urls/" + GC.UrlList);

        public List<UrlDto> GetList(SessionEnt session)
        {

            var list = new List<UrlDto>
            {
                new UrlDto { Key = GC.UrlOrgList,   Url = UrlOrg + "list" },
                new UrlDto { Key = GC.UrlOrgGet,    Url = UrlOrg + "get" },
                new UrlDto { Key = GC.UrlOrgUpdate, Url = UrlOrg + "update" },
                new UrlDto { Key = GC.UrlOrgDef,    Url = UrlOrg + "definition" },

            };

            return list;
        }

        public static string PrefixUrl(string url) => UrlBase + url;
        

    }
}
