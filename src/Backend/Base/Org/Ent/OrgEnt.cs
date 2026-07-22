using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Organisation entity
/// Required for each customer using this application
/// Created: March 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Org.Ent
{
    public class OrgEnt : BaseEncode
    {
        public int Nr { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset Updated { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public string LangCode { get; set; }
        public int LangLabelVariant { get; set; }
        public string? Icon { get; set; }
        public string? ApiKey { get; set; }
        public int Mfa { get; set; }
        public bool IsRememberMeEnabled { get; set; }
        public bool IsMasqueradeEnabled { get; set; }
        public bool IsPasswordResetEnabled { get; set; }
        public bool IsSignupEnabled { get; set; }
        public bool IsEmailRequired { get; set; }
        public bool IsEmailVerified { get; set; }

        /// <summary>
        /// If true, emails sent to users will be in HTML format. If false, the email will be in plain text format.
        /// </summary>
        public bool IsEmailHtml { get; set; }

        public OrgEnc Encoding { get; set; }

        public override void Decode()
        {
            Encoding = Decode<OrgEnc>();
        }
        public override void Encode()
        {
            Encode(Encoding);
        }
    }
}
