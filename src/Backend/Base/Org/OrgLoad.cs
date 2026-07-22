using GC = Backend.GlobalConstants;
using Npgsql;

/// <summary>
/// Utility class to load org entities.
/// Used by singleton service and other services (keeps the DRY princple)
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Org
{
    public class OrgLoad : SqlUtils
    {
        static public OrgEnt Load(NpgsqlDataReader r)
        {
            var org = new OrgEnt();
            org.Nr = GetInt(r, "nr");
            org.Code = GetCode(r);
            org.Description = GetDescription(r);
            org.Icon = GetStringNull(r, "icon");
            org.ApiKey = GetStringNull(r, "apiKey");
            org.Updated = GetUpdated(r);
            org.Version = GetVersion(r);
            org.IsActive = IsActive(r);
            org.LangLabelVariant = GetInt(r, "langLabelVariant");
            org.Encoded = GetEncoded(r);
            org.Mfa = GetInt(r, "mfa");
            org.IsRememberMeEnabled = GetBoolean(r, "isRememberMeEnabled");
            org.IsMasqueradeEnabled = GetBoolean(r, "isMasqueradeEnabled");
            org.IsPasswordResetEnabled = GetBoolean(r, "isPasswordResetEnabled");
            org.IsSignupEnabled = GetBoolean(r, "isSignupEnabled");
            org.IsEmailRequired = GetBoolean(r, "isEmailRequired");
            org.IsEmailVerified = GetBoolean(r, "isEmailVerified");
            org.IsEmailHtml = GetBoolean(r, "isEmailHtml");
            org.Decode();

            if (org.LangCode == null) org.LangCode = GC.LangCodeDefault;

            return org;
        }

        static public OrgDto Load(OrgEnt org, List<OrgLangDto> langDtos)
        {
            var enc = org.Encoding;
            return new OrgDto
            {
                Nr = org.Nr,
                Code = org.Code,
                Description = org.Description,
                Icon = org.Icon,
                Updated = org.Updated,
                Version = org.Version,
                IsActive = org.IsActive,
                LangCode = org.LangCode,
                LangLabelVariant = org.LangLabelVariant,
                Languages = langDtos,

                PasswordRule = new PasswordRuleDto
                {
                    MinLength = enc.PasswordRule.MinLength,
                    MaxLength = enc.PasswordRule.MaxLength,
                    IsMixedCase = enc.PasswordRule.IsMixedCase,
                    IsNonLetter = enc.PasswordRule.IsSpecial,
                    IsNumber = enc.PasswordRule.IsNumber,
                },

                LoginAttemptRule = new LoginAttemptRuleDto
                {
                    WarningAttempts = enc.LoginAttemptRule.WarningAttempts,
                    LockoutAttempts = enc.LoginAttemptRule.LockoutAttempts,
                    WarningLockoutMinutes = enc.LoginAttemptRule.WarningLockoutMinutes,
                    LockoutPasswordResetLink = enc.LoginAttemptRule.LockoutPasswordResetLink,
                    WarningPasswordResetLink = enc.LoginAttemptRule.WarningPasswordResetLink,
                }
            };
        }

    }
}
