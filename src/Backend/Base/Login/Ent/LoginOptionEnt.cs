using System.ComponentModel.DataAnnotations.Schema;

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
        public string UrlSuffix { get; set; }
        public bool IsActive { get; set; }
        public int OrgNr { get; set; }
        public string OrgNrs { get; set; }
        public string LangCode { get; set; }
        public int LangLabelVariant { get; set; }
        public string LangCodes { get; set; }
        public bool IsMfa { get; set; }
        public int SuccessAction { get; set; }
        public bool IsRememberMe { get; set; }
        public bool IsForgot { get; set; }
        public bool IsSelfRegistration { get; set; }
        public bool IsMasquerade { get; set; }

        [NotMapped]
        public bool IsService { get; set; }
    }
}
