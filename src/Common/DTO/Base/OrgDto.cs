namespace Common.DTO.Base
{
    public class OrgDto : _BaseDto
    {
        public int Nr { get; set; }
        public string? Icon { get; set; }
        public string LangCode { get; set; }
        public int LangLabelVariant { get; set; }
        public string? ApiKey { get; set; }
        public int Mfa { get; set; }
        public bool IsRememberMeEnabled { get; set; }
        public bool IsMasqueradeEnabled { get; set; }
        public bool IsPasswordResetEnabled { get; set; }
        public bool IsSignupEnabled { get; set; }
        public bool IsEmailRequired { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsEmailHtml { get; set; }

        public List<OrgLangDto> Languages { get; set; }
        public PasswordRuleDto PasswordRule { get; set; }
        public LoginAttemptRuleDto LoginAttemptRule { get; set; }
    }

    public class OrgLangDto
    {
        public string? LangCode { get; set; }
        public bool IsVisible { get; set; }
        public bool IsEditable { get; set; }
    }

    public class PasswordRuleDto
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public bool IsMixedCase { get; set; }
        public bool IsNumber { get; set; }
        public bool IsNonLetter { get; set; }
    }

    public class LoginAttemptRuleDto
    {
        public int WarningAttempts { get; set; } 
        public bool WarningPasswordResetLink { get; set; }
        public int WarningLockoutMinutes { get; set; }
        public int LockoutAttempts { get; set; } 
        public bool LockoutPasswordResetLink { get; set; }
    }

}
