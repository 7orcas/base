using System.Security;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Session.Ent
{
    /// <summary>
    /// Session containing relevant objects for the logged in user
    /// Note a user can have multiple sessions open
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>April 5, 2025</created>
    /// <license>**Licence**</license>
    public class SessionEnt
    {
        public string Key { get; set; }
        public int SourceApp {  get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public bool IsService { get; set; }
        public bool IsAdminUser { get; set; }
        public bool IsAdminLang { get; set; }


        /****************************************************
         * Session attributes
         ****************************************************/
        public string LangCodeCurrent { get; set; }
        public string Username { get; set; }
        public TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
        public int OrgNr { get; set; }
        public int OrgNrDefault { get; set; }
        public long? MasqueradeId { get; set; }
        public long UserAccountId { get; set; }
        public int? Classification { get; set; }


        public List<ConfigLanguage> Languages { get; set; }
        public List<PermissionCrudEnt> Permissions { get; set; }

        //Api testing only
        public string? BearerToken { get; set; }

        /****************************************************
         * Convienence objects loaded in SessionMiddleware
         ****************************************************/
        public OrgEnt Org { get; set; }
        public Dictionary<string, string> Labels { get; set; }


        /****************************************************
         * Convienence methods
         ****************************************************/

        /// <summary>
        /// Return the user's crud value for the permission
        /// Return null if permission not loaded
        /// </summary>
        /// <param name="perm"></param>
        /// <returns></returns>
        public string GetUserPermissionCrud (int permNr)
        {
            var p = Permissions.FirstOrDefault(p => p.Nr == permNr);
            if (p == null) return null;
            return p.Crud;
        }

        public bool IsUserPermission(int permNr)
        {
            if (IsService) return true;
            var p = Permissions.FirstOrDefault(p => p.Nr == permNr);
            return p != null;
        }
        public bool IsUserPermissionUpdate(int permNr)
        {
            if (IsService) return true;
            var p = Permissions.FirstOrDefault(p => p.Nr == permNr);
            return p != null && p.Crud.Contains(GC.CrudUpdate);
        }
        public bool IsUserPermissionCreate(int permNr)
        {
            if (IsService) return true;
            var p = Permissions.FirstOrDefault(p => p.Nr == permNr);
            return p != null && p.Crud.Contains(GC.CrudCreate);
        }
        public bool IsUserPermissionDelete(int permNr)
        {
            if (IsService) return true;
            var p = Permissions.FirstOrDefault(p => p.Nr == permNr);
            return p != null && p.Crud.Contains(GC.CrudDelete);
        }
    }
}
