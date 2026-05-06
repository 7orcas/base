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
        public long Id { get; set; }
        public string Userid { get; set; }
        public string Password { get; set; }
        public int? Attempts { get; set; }
        public DateTime Lastlogin { get; set; }
        public bool IsActive { get; set; }
        //Update note: Add to service


        public bool IsService () => Id == GC.ServiceLoginId;
        

        /*
         * Special service account
         * Account is not in database
         */
        public static LoginEnt GetServiceLogin()
        {
            return new LoginEnt
            {
                Id = GC.ServiceLoginId,
                Userid = "service",
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
        public bool Valid { get; set; } = false;

        public string TokenKey { get; set; }
        public string Token { get; set; }
        public string MainUrl { get; set; }
        public string LangCode { get; set; }
        public string ErrorMessage { get; set; }
    }

}
