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
            var urls = base.GetList(session);

            base.AddOrReplace(urls, "MainMenu", "Menu/" + GC.HF + "mainmenu");

            if (session.IsUserPermission(GC.PerCodeTemplate))
            {
                Add(urls, "CodeTemplateSearch", "CodeTemplate/search");
                Add(urls, "CodeTemplateList", "CodeTemplate/list");
                Add(urls, "CodeTemplateGet", "CodeTemplate/get");
            }

            if (session.IsUserPermissionUpdate(GC.PerCodeTemplate))
            {
                Add(urls, "CodeTemplateDef", "CodeTemplate/def");
                Add(urls, "CodeTemplateUpdate", "CodeTemplate/update");
            }

            if (session.IsUserPermissionCreate(GC.PerCodeTemplate))
            {
                Add(urls, "CodeTemplateDef", "CodeTemplate/def");
                Add(urls, "CodeTemplateNew", "CodeTemplate/new");
            }
            
            if (session.IsUserPermission(GC.PerTrail))
            {
                Add(urls, "TrailSearch", "Trail/search");
                Add(urls, "TrailList", "Trail/list");
                Add(urls, "TrailGet", "Trail/get");
            }

            if (session.IsUserPermissionUpdate(GC.PerTrail))
            {
                Add(urls, "TrailDef", "Trail/def");
                Add(urls, "TrailUpdate", "Trail/update");
            }

            if (session.IsUserPermissionCreate(GC.PerTrail))
            {
                Add(urls, "TrailDef", "Trail/def");
                Add(urls, "TrailNew", "Trail/new");
            }

            if (session.IsUserPermission(GC.PerTrailGroup))
            {
                Add(urls, "TrailGroupSearch", "TrailGroup/search");
                Add(urls, "TrailGroupList", "TrailGroup/list");
                Add(urls, "TrailGroupGet", "TrailGroup/get");
            }

            if (session.IsUserPermissionUpdate(GC.PerTrailGroup))
            {
                Add(urls, "TrailGroupDef", "TrailGroup/def");
                Add(urls, "TrailGroupUpdate", "TrailGroup/update");
            }

            if (session.IsUserPermissionCreate(GC.PerTrailGroup))
            {
                Add(urls, "TrailGroupDef", "TrailGroup/def");
                Add(urls, "TrailGroupNew", "TrailGroup/new");
            }

            //INSERT_GENERATED_CODE_HERE

            return urls;
        }



    }
}
