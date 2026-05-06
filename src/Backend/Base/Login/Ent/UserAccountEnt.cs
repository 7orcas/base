using GC = Backend.GlobalConstants;

/// <summary>
/// User login entity to an organisation id
/// Users can have access to different organisations (each org must have a separate user account)
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Login.Ent
{
    public class UserAccountEnt : BaseEncode
    {
        public long Id { get; set; }
        public long LoginId { get; set; }
        public int orgNr { get; set; }
        public string? LangCode { get; set; }
        public int? Classification {  get; set; }
        public DateTime Lastlogin { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        //Update note: Add to service


        public string Userid { get; set; }
        public override void Decode() { }
        public override void Encode() { }


        public List<PermissionCrudEnt> Permissions { get; set; }

        public bool IsSystemAdmin { get; set; } = false;
        public bool IsCurrentLanguageAdmin { get; set; } = false;
        public bool IsActiveLanguageAdmin { get; set; } = false;

        public bool IsLanguageAdmin()
        {
            return IsCurrentLanguageAdmin || IsActiveLanguageAdmin || IsService();
        }
        public bool IsService() => LoginId == GC.ServiceLoginId;

        /*
         * Special service account
         * Account does not have to be in the database
         */
        public static UserAccountEnt GetServiceAccount(int orgNr)
        {
            return new UserAccountEnt
            {
                Id = GC.ServiceAccountId,
                LoginId = GC.ServiceLoginId,
                orgNr = orgNr,
                LangCode = GC.LangCodeDefault,
                Lastlogin = DateTime.Now,
                IsActive = true,
                IsAdmin = true
            };
        }
    }
}
