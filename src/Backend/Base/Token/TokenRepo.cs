
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


        public async Task<RefreshToken?> LoadRefreshToken(long id)
        {
            RefreshToken token = null;
            await Sql.Run(
                    "SELECT * FROM cntrl.tokenrefresh "
                    + "WHERE id = @id ",
                    r => {
                        token = RefreshTokenLoad.Load(r);
                    },
                    new NpgsqlParameter("@id", id)
                );
            return token;
        }



        public async Task<RefreshToken> SaveRefreshToken(RefreshToken token)
        {
            var sql = @"
                    INSERT INTO cntrl.tokenrefresh
                        (token, createdbyip, username, sessionkey, orgnr, expires)
                    VALUES
                        (@Token, @CreatedByIp, @Username, @SessionKey, @OrgNr, @Expires);";

            var id = await Sql.ExecuteAndReturnId(sql, new
            {
                token.Token,
                token.CreatedByIp,
                token.Username,
                token.SessionKey,
                token.OrgNr,
                Expires = token.Expires
            });

            token.Id = id;
            return token;
        }


        public async Task RevokeRefreshToken(long id, string revokedBy)
        {
            await Sql.Execute(
                    "UPDATE cntrl.tokenrefresh " +
                    "SET revoked = CURRENT_TIMESTAMP, " +
                        "revokedBy = '" + revokedBy + "' " +
                    "WHERE id = " + id.ToString());
        }


    }
}
