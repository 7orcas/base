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
        public bool IsNonLetter { get; set; } = true;
    }

}
