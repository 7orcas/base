using System.Text;
using Microsoft.IdentityModel.Tokens; 


namespace Backend.Base.Token.Ent
{
    public class TokenParameters
    {
        static public string _Key { get; set; } = "ThisIsASecureLongEnoughKeyZ1234567890JohnStewartWasHereIn2025";
        static public string _Issuer { get; set; } = "BlueIssuer";
        static public string _Audience { get; set; } = "BlueAudience";

        static public bool _ValidateIssuer { get; set; } = true;
        static public bool _ValidateIssuerSigningKey { get; set; } = true;
        static public bool _ValidateAudience { get; set; } = true;
        static public bool _ValidateLifetime { get; set; } = true;


        static public TokenValidationParameters GetParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = _ValidateIssuer,
                ValidateAudience = _ValidateAudience,
                ValidateLifetime = _ValidateLifetime,
                ValidateIssuerSigningKey = _ValidateIssuerSigningKey,
                ValidIssuer = _Issuer,
                ValidAudience = _Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Key))
            };
        }
}
}
