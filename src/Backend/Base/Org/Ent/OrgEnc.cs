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

        /// <summary>
        /// User password rules for this org.
        /// </summary>
        [NotMapped]
        public PasswordRule PasswordRule { get; set; } = new PasswordRule();
        
        /// <summary>
        /// Login attempt rules for this org.
        /// </summary>
        [NotMapped]
        public LoginAttemptRule LoginAttemptRule { get; set; } = new LoginAttemptRule();

        /// <summary>
        /// The number of days a signup registration email address must be verified before it expires. (default is 30 days).
        /// </summary>
        [NotMapped]
        public int SignupExpiryDays { get; set; } = 30;

        /// <summary>
        /// The date format for this org (default is "dd MMM yyyy").
        /// </summary>
        [NotMapped]
        public string DateFormatDMY { get; set; } = "dd MMM yyyy";

        /// <summary>
        /// The date and time format for this org (default is "dd MMM yyyy HH:mm tt").
        /// </summary>
        [NotMapped]
        public string DateTimeFormatDMY { get; set; } = "dd MMM yyyy HH:mm tt";

        /// <summary>
        /// The IT support contact for users. (default is empty string).
        /// </summary>
        [NotMapped]
        public string SupportIT { get; set; } = "";
    }

    /// <summary>
    /// Represents a language with its code and editability properties.
    /// </summary>
    public class Language 
    {
        /// <summary>
        /// The language code for this language (e.g., "en" for English, "fr" for French).
        /// </summary>
        public string LangCode { get; set; }

        /// <summary>
        /// If true, the language is visible to users. If false, the language is hidden from users. (default is true).
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// If true, the language value can be edited. If false, the language value is read-only. (default is false).
        /// </summary>
        public bool IsEditable { get; set; } = false;
    }

    /// <summary>
    /// Represents the password rules for an org
    /// </summary>
    public class PasswordRule
    {
        /// <summary>
        /// The minimum length for passwords. (default is 8 characters).
        /// </summary>
        public int MinLength { get; set; } = 8;

        /// <summary>
        /// The maximum length for passwords. (default is 30 characters).
        /// </summary>
        public int MaxLength { get; set; } = 30;

        /// <summary>
        /// If true, passwords must contain both uppercase and lowercase letters. (default is true).
        /// </summary>
        public bool IsMixedCase { get; set; } = true;

        /// <summary>
        /// If true, passwords must contain at least one number. (default is true).
        /// </summary>
        public bool IsNumber { get; set; } = true;

        /// <summary>
        /// If true, passwords must contain at least one special character. (default is true).
        /// </summary>
        public bool IsSpecial { get; set; } = true;
    }

    /// <summary>
    /// Represents the login attempt rules for an org
    /// </summary>
    public class LoginAttemptRule
    {
        /// <summary>
        /// The number of failed login attempts allowed before warning the user and allowing other action. (default is 3).
        /// </summary>
        public int WarningAttempts { get; set; } = 3;

        /// <summary>
        /// If true, the  password reset link is enabled and users will be able to reset their password after reaching the warning number of failed login attempts. (default is true).
        /// </summary>
        public bool WarningPasswordResetLink { get; set; } = true;

        /// <summary>
        /// The number of minutes a user will be temporarily locked out for exceeding the warning number of failed login attempts. (default is 15).
        /// </summary>
        public int WarningLockoutMinutes { get; set; } = 1;

        /// <summary>
        /// The maximum number of failed login attempts before a user is locked out. (default is 5).
        /// </summary>
        public int LockoutAttempts { get; set; } = 5;

        /// <summary>
        /// If true, the  password reset link is enabled and users will be able to reset their password after reaching the lockout number of failed login attempts. (default is false).
        /// </summary>
        public bool LockoutPasswordResetLink { get; set; } = false;
    }


}
