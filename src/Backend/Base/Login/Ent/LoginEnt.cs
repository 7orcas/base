using GC = Backend.GlobalConstants;

/// <summary>
/// User login entity
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Login.Ent
{

    public class LoginEnt
    {
        public const int UsernameMaxLength = 40;
        public const int PasswordMaxLength = 100;
        public const int EmailMaxLength = 100;

        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string Password { get; set; }
        public int OrgNrDefault { get; set; }
        public string LangCode { get; set; }
        public int? Attempts { get; set; }
        public DateTimeOffset Updated { get; set; }
        public DateTimeOffset? Lastlogin { get; set; }
        public bool IsActive { get; set; }

        //Update note: Add to service
        public bool IsService () => Id == GC.ServiceLoginId;

        public string? MfaSecret { get; set; }
        public bool IsMfaEnabled { get; set; }
        public bool IsMfaRequired { get; set; }

        /*
         * Special service account
         * Account is not in database
         */
        public static LoginEnt GetServiceLogin()
        {
            return new LoginEnt
            {
                Id = GC.ServiceLoginId,
                Username = "service",
                Password = "",
                Attempts = 0,
                Lastlogin = DateTime.Now,
                IsActive = true
            };
        }

        //Login Repsonse variables
        public LoginResponse Response { get; set; } = new LoginResponse();
    }

    public class LoginResponse
    {
        public bool IsValid { get; set; } = false;
        public string TokenKey { get; set; }
        public string MainUrl { get; set; }
        public string LangCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsMfaRequired { get; set; } = false;
        public bool IsMfaEnabled { get; set; } = false;
        public bool IsEmailVerified { get; set; } = false;
    }

}
