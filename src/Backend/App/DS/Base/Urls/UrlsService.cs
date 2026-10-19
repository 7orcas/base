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


        
        private readonly string UrlMachine = PrefixUrl("Machine/");
        

        public string GetAppUrl() => PrefixUrl("Urls/" + GC.DS + GC.UrlList);

        public List<UrlDto> GetList(SessionEnt session)
        {
            var list = base.GetList(session);

            list.Add(new UrlDto { Key = "MachList", Url = UrlMachine + "list" });

            return list;
        }



    }
}
