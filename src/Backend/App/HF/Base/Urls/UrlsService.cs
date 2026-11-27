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
            
            if (session.IsUserPermission(GC.PerTrial))
            {
                Add(urls, "TrialSearch", "Trial/search");
                Add(urls, "TrialList", "Trial/list");
                Add(urls, "TrialGet", "Trial/get");
                Add(urls, "TrialConfig", "Trial/config");
            }

            if (session.IsUserPermissionUpdate(GC.PerTrial))
            {
                Add(urls, "TrialDef", "Trial/def");
                Add(urls, "TrialUpdate", "Trial/update");
            }

            if (session.IsUserPermissionCreate(GC.PerTrial))
            {
                Add(urls, "TrialDef", "Trial/def");
                Add(urls, "TrialNew", "Trial/new");
                Add(urls, "StartTrial", "Trial/starttrial");
            }

            if (session.IsUserPermission(GC.PerTrialGroup))
            {
                Add(urls, "TrialGroupSearch", "TrialGroup/search");
                Add(urls, "TrialGroupList", "TrialGroup/list");
                Add(urls, "TrialGroupGet", "TrialGroup/get");
                Add(urls, "TrialGroupMemberGet", "TrialGroup/getmembers");
            }

            if (session.IsUserPermissionUpdate(GC.PerTrialGroup))
            {
                Add(urls, "TrialGroupDef", "TrialGroup/def");
                Add(urls, "TrialGroupUpdate", "TrialGroup/update");
            }

            if (session.IsUserPermissionCreate(GC.PerTrialGroup))
            {
                Add(urls, "TrialGroupDef", "TrialGroup/def");
                Add(urls, "TrialGroupNew", "TrialGroup/new");
            }

            //INSERT_GENERATED_CODE_HERE

            return urls;
        }



    }
}
