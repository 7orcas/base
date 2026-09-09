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
        public int OrgNr { get; set; }
        public LoginAccountEnt UserAccount { get; set; }
        public long? MasqueradeId { get; set; }
        public ConfigUser UserConfig { get; set; }
        public int SourceApp {  get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");

        //Loaded in SessionMiddleware
        public Dictionary<string, string> Labels { get; set; }
        
        //Loaded in SessionMiddleware
        public OrgEnt Org { get; set; }

        //Api testing only
        public string? BearerToken { get; set; }

        /// <summary>
        /// Return the user's crud value for the permission
        /// Return null if permission not loaded
        /// </summary>
        /// <param name="perm"></param>
        /// <returns></returns>
        public string GetUserPermissionCrud (int permNr)
        {
            var p = UserAccount.Permissions.FirstOrDefault(p => p.Nr == permNr);
            if (p == null) return null;
            return p.Crud;
        }


        //Convience methods
        public bool IsService => UserAccount.IsService();

    }
}
