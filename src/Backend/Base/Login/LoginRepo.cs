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
                            Emailverified = GetBoolean(r, "emailverified"),
                            Password = GetString(r, "yyy"),
                            OrgNrDefault = GetInt(r, "orgnrdefault"),
                            LangCode = GetString(r, "langCode"),
                            Attempts = GetIntNull(r, "attempts"),
                            Lastlogin = GetDateTime(r, "lastlogin"),
                            IsActive = GetBoolean(r, "isActive"),
                            MfaSecret = GetStringNull(r, "mfasecret"),
                            MfaEnabled = GetBoolean(r, "mfaenabled"),
                        };
                    },
                    new NpgsqlParameter("@id", id)
                );
            }
            catch { }

            return login;
        }

        public async Task<bool> CreateSignup(LoginEnt login)
        {
            try
            {
                await Sql.ExecuteAsync(
                        "INSERT INTO base.zzz " +
                            "(xxx, yyy, email, emailverified, orgnrdefault, langcode, isactive) " +
                        "VALUES (" +
                            Insert(login.Username) +
                            Insert(login.Password) +
                            Insert(login.Email) +
                            Insert(login.Emailverified) +
                            Insert(login.OrgNrDefault) +
                            Insert(login.LangCode) +
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

        public async Task<bool> UpdateSignup(LoginEnt login)
        {
            try
            {
                await Sql.ExecuteAsync(
                        "UPDATE base.zzz " +
                        "SET " +
                            Update("emailverified", login.Emailverified) +
                            Update("isActive", login.IsActive) +
                            NoComma(Updatetime()) +
                        " WHERE id = " + login.Id
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

    }
}
