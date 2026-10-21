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


        public Dictionary<string, UrlDto> GetList(SessionEnt session)
        {
            var urls = new Dictionary<string, UrlDto>();

            Add(urls, "Logout", "Logout/logout");
            Add(urls, "MainMenu", "Menu/mainmenu");

            if (session.IsUserPermission(GC.PerOrg))
            {
                Add(urls, "OrgList",    "Org/list");
                Add(urls, "OrgGet",     "Org/get");
            }
            if (session.IsUserPermissionUpdate(GC.PerOrg))
            {
                Add(urls, "OrgUpdate",  "Org/update");
                Add(urls, "OrgDef",     "Org/def");
            }

            if (session.IsUserPermission(GC.PerUser))
            {
                Add(urls, "UserSearch", "User/search");
                Add(urls, "UserList",   "User/list");
                Add(urls, "UserGet",    "User/get");
            }
            if (session.IsUserPermissionUpdate(GC.PerUser))
            {
                Add(urls, "UserDef",    "User/def");
                Add(urls, "UserUpdate", "User/update");
            }
            if (session.IsUserPermissionCreate(GC.PerUser))
            {
                Add(urls, "UserDef", "User/def");
                Add(urls, "UserNew", "User/new");
            }

            if (session.IsUserPermission(GC.PerRole))
            {
                Add(urls, "RoleList",  "Role/list");
                Add(urls, "RoleGet",   "Role/get");
                Add(urls, "UserRoles", "Role/userroles");
            }
            if (session.IsUserPermissionUpdate(GC.PerRole))
            {
                Add(urls, "RoleUpdate", "Role/update");
            }

            if (session.IsUserPermission(GC.PerLabel))
            {
                Add(urls, "LabelClientList", "Label/clientlist");
                Add(urls, "LabelRelatedList", "Label/relatedlist");
            }

            if (session.IsUserPermission(GC.PerConfig))
            {
            }
Add(urls, "ClientConfig", "Config/clientConfig");

            if (session.IsUserPermission(GC.PerPerm))
            {
                Add(urls, "PermissionList", "Permission/list");
                Add(urls, "PermissionUserList", "Permission/userlist");
                Add(urls, "PermissionUserListEff", "Permission/userlisteff");
            }

            if (session.IsUserPermission(GC.PerAudit))
            {
                Add(urls, "AuditSearch", "Audit/search");
                Add(urls, "AuditList", "Audit/list");
                Add(urls, "AuditGet", "Audit/get");
            }

            return urls;
        }

        public void Add(Dictionary<string, UrlDto> urls, string key, string url)
        {
            if (!urls.ContainsKey(key))
                urls.Add(key, new UrlDto { Key = key, Url = PrefixUrl(url) });
        }
        public void AddOrReplace(Dictionary<string, UrlDto> urls, string key, string url)
        {
            if (urls.ContainsKey(key))
                urls.Remove(key);
            urls.Add(key, new UrlDto { Key = key, Url = PrefixUrl(url) });
        }

        public string GetAppUrl() => PrefixUrl("Urls/" + GC.UrlList);
        public static string PrefixUrl(string url) => "api/" + url;
        

    }
}
