using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Encoded class of Organisation entity
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Org.Ent
{
    public class OrgEnc
    {
        /// <summary>
        /// Language codes visible to this org.
        /// </summary>
        [NotMapped]
        public List<Language> Languages { get; set; } = new List<Language>();

        public int MaxNumberLoginAttempts { get; set; } = 3;

        public PasswordRule PasswordRule { get; set; } = new PasswordRule();

        public bool IsEmailHtml { get; set; } = true;

        public int SignupExpiryDays { get; set; } = 30; // Number of days a signup registration email address must be verified before it expires
        public string DateTimeFormatDMY { get; set; } = "dd MMM yyyy";
        public string SupportIT { get; set; } = "The IT support team";
    }

    public class Language 
    {
        public string LangCode { get; set; }
        public bool IsReadonly { get; set; } = true;
        public bool IsEditable { get; set; } = false;
    }

    public class PasswordRule
    {
        public int MinLength { get; set; } = 8;
        public int MaxLength { get; set; } = 30;
        public int MaxAttempts { get; set; } = 3;
        public bool IsMixedCase { get; set; } = true;
        public bool IsNumber { get; set; } = true;
        public bool IsSpecial { get; set; } = true;
    }

}
