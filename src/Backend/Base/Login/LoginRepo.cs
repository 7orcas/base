using Npgsql;

namespace Backend.Base.Login
{
    public class LoginRepo : BaseRepo, LoginRepoI
    {
        public LoginRepo(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public async Task<bool> IsUniqueUsername(string username)
        {
            return await IsUnique(username, "xxx");
        }

        public async Task<bool> IsUniqueEmail(string email)
        {
            return await IsUnique(email, "email");
        }


        public async Task<LoginEnt?> GetLoginByUsername(string username)
        {
            var login = null as LoginEnt;

            long? id = null;
            try
            {
                //ToDo Log
                if (!ValidateParameter(username))
                    throw new Exception();



                await Sql.Run(
                    "SELECT id FROM base.zzz " +
                        "WHERE xxx = @username ",
                    r =>
                    {
                        id = GetId(r);
                    },
                    new NpgsqlParameter("@username", username)
                );

                if (id != null)
                    login = await GetLoginById(id.Value);
            }
            catch { }

            return login;
        }

        public async Task<LoginEnt?> GetLoginByEmail(string email)
        {
            long? id = null;
            try
            {

                await Sql.Run(
                    "SELECT id FROM base.zzz " +
                        "WHERE email = @email ",
                    r =>
                    {
                        id = GetId(r);
                    },
                    new NpgsqlParameter("@email", email)
                );
            }
            catch { }

            if (id == null)
                return null;

            return await GetLoginById(id.Value);
        }


        public async Task<LoginEnt?> GetLoginById(long id)
        {
            var login = null as LoginEnt;

            try
            {
                await Sql.Run(
                    "SELECT * FROM base.zzz " +
                        "WHERE id = @id ",
                    r =>
                    {
                        login = new LoginEnt
                        {
                            Id = GetId(r),
                            Username = GetString(r, "xxx"),
                            Email = GetString(r, "email"),
                            IsEmailVerified = GetBoolean(r, "isEmailVerified"),
                            Password = GetString(r, "yyy"),
                            OrgNrDefault = GetInt(r, "orgnrdefault"),
                            LangCode = GetString(r, "langCode"),
                            Attempts = GetIntNull(r, "attempts"),
                            Updated = GetDateTime(r, "updated"),
                            Lastlogin = GetDateTimeNull(r, "lastlogin"),
                            IsActive = GetBoolean(r, "isActive"),
                            MfaSecret = GetStringNull(r, "mfasecret"),
                            IsMfaEnabled = GetBoolean(r, "isMfaEnabled"),
                            IsMfaRequired = GetBoolean(r, "isMfaRequired"),
                        };
                    },
                    new NpgsqlParameter("@id", id)
                );
            }
            catch { }

            return login;
        }

        public async Task<UserAccountEnt?> GetAccount(long loginId, int orgNr)
        {
            var account = null as UserAccountEnt;

            try
            {
                await Sql.Run(
                    "SELECT * FROM base.userAcc " +
                        "WHERE zzzId = @zzzId " +
                        "AND orgNr = @orgNr",
                    r =>
                    {
                        account = new UserAccountEnt
                        {
                            Id = GetId(r),
                            LoginId = GetId(r, "zzzId"),
                            OrgNr = GetOrgNr(r),
                            Lastlogin = GetDateTimeNull(r, "lastlogin"),
                            IsActive = IsActive(r),
                            IsAdmin = GetBoolean(r, "isAdmin"),
                            Classification = GetIntNull(r, "classification")
                        };
                    },
                    new NpgsqlParameter("@zzzId", loginId),
                    new NpgsqlParameter("@orgNr", orgNr)
                );
            }
            catch { }

            return account;
        }

        public async Task<bool> SetAttempts(long id, int attempts)
        {
            await Sql.ExecuteAsync(
                   "UPDATE base.zzz "
                   + "SET Attempts = " + attempts + " "
                   + "WHERE id = " + id
               );
            return true;
        }

        public async Task UpdateLastLogin(long id, long accountId)
        {
            await Sql.ExecuteAsync(
                   "UPDATE base.zzz "
                   + "SET lastlogin = CURRENT_TIMESTAMP "
                   + "WHERE id = " + id
               );
            await Sql.ExecuteAsync(
                   "UPDATE base.useracc "
                   + "SET lastlogin = CURRENT_TIMESTAMP "
                   + "WHERE id = " + accountId
               );
        }

        public async Task<bool> CreateSignup(LoginEnt login)
        {
            try
            {
                var id = await Sql.ExecuteAndReturnIdAsync(
                        "INSERT INTO base.zzz " +
                            "(xxx, yyy, email, isEmailVerified, orgnrDefault, langcode, isActive) " +
                        "VALUES (" +
                            Insert(login.Username) +
                            Insert(login.Password) +
                            Insert(login.Email) +
                            Insert(login.IsEmailVerified) +
                            Insert(login.OrgNrDefault) +
                            Insert(login.LangCode) +
                            NoComma(Insert(login.IsActive)) +
                            ")"
                );
                login.Id = id;

                await Sql.ExecuteAsync(
                        "INSERT INTO base.useracc " +
                            "(zzzid, orgnr, isActive) " +
                        "VALUES (" +
                            Insert(id) +
                            Insert(login.OrgNrDefault) +
                            NoComma(Insert(login.IsActive)) +
                            ")"
                );

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> VerifySignup(LoginEnt login)
        {
            try
            {
                await Sql.ExecuteAsync(
                        "UPDATE base.zzz " +
                        "SET " +
                            Update("isEmailVerified", login.IsEmailVerified) +
                            Update("isActive", login.IsActive) +
                            NoComma(UpdateDatetimeNow()) +
                        " WHERE id = " + login.Id
                );
                await Sql.ExecuteAsync(
                        "UPDATE base.useracc " +
                        "SET " +
                            Update("isActive", login.IsActive) +
                            NoComma(UpdateDatetimeNow()) +
                        " WHERE zzzid = " + login.Id
                );
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        private async Task<bool> IsUnique(string value, string field)
        {
            try
            {
                long id = -987654;
                await Sql.Run(
                    "SELECT id FROM base.zzz " +
                        "WHERE " + field + " = @value ",
                    r =>
                    {
                        id = GetId(r);
                    },
                    new NpgsqlParameter("@value", value)
                );

                if (id == -987654)
                    return true;
            }
            catch
            {
                _log.Error("IsUnique value {value} field {field}", value, field);
            }
            return false;
        }

        public async Task<bool> SetMfaKey(long id, string key)
        {
            await Sql.ExecuteAsync(
                   "UPDATE base.zzz "
                   + "SET mfasecret = '" + key + "' "
                   + "WHERE id = " + id
               );
            return true;
        }

        public async Task<bool> EnableMfa(long id)
        {
            await Sql.ExecuteAsync(
                   "UPDATE base.zzz "
                   + "SET ismfaenabled = true "
                   + "WHERE id = " + id
               );
            return true;
        }


    }
}
