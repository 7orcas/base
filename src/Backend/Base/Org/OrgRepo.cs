using Backend.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Org
{
    /// <summary>
    /// Manage Organisation  
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>September, 2026</created>
    /// <license>**Licence**</license>
    public class OrgRepo : BaseRepo, OrgRepoI
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public OrgRepo(AppDbContext context,
            IServiceProvider serviceProvider,
            IMemoryCache memoryCache)
            : base(serviceProvider)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        public async Task<VersionInfo?> GetVersion(int nr)
        {
            return await GetVersion(nr, "base.org");
        }

        public async Task<List<OrgEnt>> GetList()
        {
            var sql = "SELECT nr, code, descr as description, isActive "
                       + "FROM base.org "
                       + " ORDER BY code";

            return await GetList<OrgEnt>(sql);
        }


        public async Task<OrgEnt?> GetByNr(int nr)
        {
            try
            {
                return await _context.Orgs
                    .FirstOrDefaultAsync(x => x.Nr == nr);

            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving user with Nr {nr}", ex);
                return null;
            }
        }

        public async Task<OrgEnt?> Update(OrgDto orgDto)
        {
            var org = null as OrgEnt;

            if (orgDto.IsNew())
                org = new OrgEnt();
            else
                org = await GetByNr(orgDto.Nr);

            if (org == null)
                return null;

            org.Code = orgDto.Code;
            org.Description = orgDto.Description;
            org.IsActive = orgDto.IsActive;
            org.Mfa = orgDto.Mfa;
            org.IsRememberMeEnabled = orgDto.IsRememberMeEnabled;
            org.IsMasqueradeEnabled = orgDto.IsMasqueradeEnabled;
            org.IsPasswordResetEnabled = orgDto.IsPasswordResetEnabled;
            org.IsSignupEnabled = orgDto.IsSignupEnabled;
            org.IsEmailRequired = orgDto.IsEmailRequired;
            org.IsEmailHtml = orgDto.IsEmailHtml;
            org.LangCode = orgDto.LangCode;
            org.LangLabelVariant = orgDto.LangLabelVariant;

            VersionIncrement(org);

            var langs = new List<Language>();
            foreach (var langDto in orgDto.Languages)
            {
                langs.Add(new Language
                {
                    LangCode = langDto.LangCode,
                    IsVisible = langDto.IsReadonly,
                    IsEditable = langDto.IsEditable,
                });
            }

            var pw = new PasswordRule()
            {
                MinLength = orgDto.PasswordRule.MinLength,
                MaxLength = orgDto.PasswordRule.MaxLength,
                IsMixedCase = orgDto.PasswordRule.IsMixedCase,
                IsSpecial = orgDto.PasswordRule.IsNonLetter,
                IsNumber = orgDto.PasswordRule.IsNumber,
            };

            var attempts = new LoginAttemptRule()
            {
                WarningAttempts = orgDto.LoginAttemptRule.WarningAttempts,
                LockoutAttempts = orgDto.LoginAttemptRule.LockoutAttempts,
                WarningLockoutMinutes = orgDto.LoginAttemptRule.WarningLockoutMinutes,
                LockoutPasswordResetLink = orgDto.LoginAttemptRule.LockoutPasswordResetLink,
                WarningPasswordResetLink = orgDto.LoginAttemptRule.WarningPasswordResetLink,
            };

            org.Encoding = new OrgEnc
            {
                Languages = langs,
                PasswordRule = pw,
                LoginAttemptRule = attempts,
            };
            org.Encode();

            if (orgDto.IsNew())
                _context.Orgs.Add(org);

            await _context.SaveChangesAsync();
            
            _memoryCache.Set(GC.CacheKeyOrgPrefix + org.Nr, org);
            
            return org;

        }


    }
}
