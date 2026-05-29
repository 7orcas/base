/// <summary>
/// Login options entity
/// Created: May 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
namespace Backend.Base.Login.Ent
{
    public class LoginOptionEnt
    {
        public int LoginNr { get; set; }
        public bool IsActive { get; set; }
        public string OrgNrs { get; set; }
        public string LangCode { get; set; }
        public int LangLabelVariant { get; set; }
        public string LangCodes { get; set; }
        public int MFA { get; set; }
        public bool RememberMe { get; set; }
        public bool Forgot { get; set; }
        public bool SelfRegistration { get; set; }
    }
}
