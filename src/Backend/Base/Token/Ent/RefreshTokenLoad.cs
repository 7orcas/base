using Backend.Base.Token.Ent;
using Npgsql;
using GC = Backend.GlobalConstants;

/// <summary>
/// Utility class to load Refresh Token entities.
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Token
{
    public class RefreshTokenLoad : SqlUtils
    {
        static public RefreshToken Load(NpgsqlDataReader r)
        {
            var token = new RefreshToken();
            token.Id = GetId(r);
            token.Token = GetGuid(r, "Token");
            token.Created = GetDateTime(r, "Created");
            token.Expires = GetDateTime(r, "Expires");
            token.CreatedByIp = GetString(r, "CreatedByIp");
            token.Revoked = GetDateTimeNull(r, "Revoked");
            token.Username = GetString(r, "Username");
            token.SessionKey = GetString(r, "SessionKey");
            token.OrgNr = GetInt(r, "OrgNr");

            return token;
        }
    }
}
