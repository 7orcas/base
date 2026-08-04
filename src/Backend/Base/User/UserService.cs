using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
using Superpower.Model;
using GC = Backend.GlobalConstants;

/// <summary>
/// User methods
/// Created: August 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>


namespace Backend.Base.User
{
    public class UserService: BaseService, UserServiceI
    {
        private readonly LabelServiceI _labelService;
        
        public UserService(IServiceProvider serviceProvider,
            LabelServiceI labelService) 
            : base(serviceProvider) 
        {
            _labelService = labelService;
        }

        public async Task<List<UserEnt>> GetUserList()
        {
            var list = new List<UserEnt>();
            await Sql.Run(
                    "SELECT * FROM base.zzz ",
                    r => {
                        var user = new UserEnt();
                        user.Id = GetId(r);
                        user.Username = GetString(r, "xxx");
                        list.Add(user);
                    }
                );
            return list;
        }

        public async Task<UserEnt> GetUser(long id)
        {
            try
            {
                var user = new UserEnt();
                await Sql.Run(
                    "SELECT * FROM base.zzz "
                    + "WHERE id = @id ",
                    r =>
                    {
                        user = new UserEnt() 
                        { 
                            Id = GetId(r),
                            Username = GetString(r, "xxx")
                        };
                    },
                    new NpgsqlParameter("@id", id)
                );
                                
                return user;
            }
            catch 
            {
                return null;
            }
        }

        //public async Task UpdateOrg(OrgEnt org)
        //{
        //    org.Encode();
        //    await Sql.ExecuteAsync(
        //            "UPDATE base.org " +
        //            "SET " +
        //                Update("code", org.Code) +
        //                Update("descr", org.Description) +
        //                Update("encoded", org.Encoded) +
        //                Update("updated", org.Updated) +
        //                Update("version", org.Version + 1) +
        //                Update("isActive", org.IsActive) +
        //                Update("mfa", org.Mfa) +
        //                Update("isRememberMeEnabled", org.IsRememberMeEnabled) +
        //                Update("isMasqueradeEnabled", org.IsMasqueradeEnabled) +
        //                Update("isForgotenabled", org.IsPasswordResetEnabled) +
        //                Update("isSignupenabled", org.IsSignupEnabled) +
        //                Update("isEmailRequired", org.IsEmailRequired) +
        //                Update("isEmailHtml", org.IsEmailHtml) +
        //                Update("langCode", org.LangCode) +
        //                NoComma(Update("langLabelVariant", org.LangLabelVariant)) +
        //            " WHERE nr = " + org.Nr
        //    );
        //    _memoryCache.Set(GC.CacheKeyOrgPrefix + org.Nr, org);
        //}

        public UserDto Populate(UserEnt user)
        {
            UserDto userDto = new UserDto()
            {
                UserId = user.Id,
                Username = user.Username,
            };
            return userDto;
        }

    }
}
