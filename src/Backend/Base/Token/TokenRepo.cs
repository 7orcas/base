
using Backend.Base.Token.Ent;
using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;

namespace Backend.Base.Token
{
    public class TokenRepo : TokenRepoI
    {


        private readonly AppDbContext _context;

        public TokenRepo(AppDbContext context)
        {
            _context = context;
        }


        public async Task<RefreshToken?> LoadRefreshToken(string key)
        {
            RefreshToken token = null;
            await Sql.Run(
                    "SELECT * FROM cntrl.tokenrefresh "
                    + "WHERE token = @token ",
                    r => {
                        token = Load(r);
                    },
                    new NpgsqlParameter("@token", Guid.Parse(key))
                );
            return token;
        }

        static public RefreshToken Load(NpgsqlDataReader r)
        {
            var token = new RefreshToken();
            token.Token = SqlUtils.GetGuid(r, "token");
            token.OrgNr = SqlUtils.GetInt(r, "orgnr");
            token.Created = SqlUtils.GetDateTime(r, "created");
            token.CreatedByIp = SqlUtils.GetStringNull(r, "createdbyip");
            token.Username = SqlUtils.GetString(r, "username");
            token.Expires = SqlUtils.GetDateTime(r, "expires");
            token.Revoked = SqlUtils.GetDateTime(r, "revoked");

            return token;
        }


        public async Task SaveRefreshToken(RefreshToken token)
        {
            await Sql.Execute(
                    "INSERT INTO cntrl.tokenrefresh " +
                        "(token, createdbyip, username, sessionkey, orgnr, expires) " +
                    "VALUES (" +
                        "'" + token.Token + "'," +
                        "'" + token.CreatedByIp + "'," +
                        "'" + token.Username + "'," +
                        "'" + token.SessionKey + "'," +
                        token.OrgNr + "," +
                        "'" + token.Expires.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                        ")");
        }


        public async Task RevokeRefreshToken(RefreshToken token)
        {
            await Sql.Execute(
                    "UPDATE cntrl.tokenrefresh SET revoked = CURRENT_TIMESTAMP WHERE token = "
                        + "'" + token.Token.ToString() + "'");
        }


    }
}
