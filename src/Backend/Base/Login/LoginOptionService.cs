using Microsoft.Extensions.Options;
using Npgsql;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Login
{
    public class LoginOptionService : BaseService, LoginOptionServiceI
    {
        private readonly LoginOptionRepoI _loginOptionRepo;
        private readonly OrgServiceI _orgService;
        private readonly LabelServiceI _labelService;

        public LoginOptionService(IServiceProvider serviceProvider,
            LoginOptionRepoI loginOptionRepo,
            OrgServiceI orgService,
            LabelServiceI labelService)
            : base(serviceProvider)
        {
            _loginOptionRepo = loginOptionRepo;
            _orgService = orgService;
            _labelService = labelService;
        }

        public async Task<LoginOptionEnt> GetLoginOptionDefault(int orgNr)
        {
            return await _loginOptionRepo.GetLoginOptions(orgNr);
        }


        //Login options for the passed in login nr
        public async Task<LoginOptionEnt> GetLoginOptions(string urlSuffix)
        {
            var isService = false;
            if (!string.IsNullOrEmpty(urlSuffix) && urlSuffix == GC.ServiceLoginUrlSuffix)
            {
                isService = true;
                urlSuffix = ""; //get default
            }

            var loginOption = await _loginOptionRepo.GetLoginOptions(urlSuffix);
            
            if (isService)
            {
                var orgList = await _orgService.GetOrgList();
                var orgs = string.Join(",", orgList.Select(o => o.Nr));
                loginOption.OrgNrs = orgs;

                var langCodeList = await _labelService.GetLangCodeList();
                var langs = string.Join(",", langCodeList.Select(l => l.Code));
                loginOption.LangCode = GC.LangCodeDefault;
                loginOption.LangLabelVariant = 0;
                loginOption.LangCodes = langs;

                loginOption.SuccessAction = GC.NavigateToFrontendServer;
                loginOption.IsMasqueradeEnabled = true;
                loginOption.IsActive = true;
            }

            return loginOption;
        }


        /// <summary>
        /// Initialise login options:
        /// 1. Set options with the default org nr
        /// 2. Remove options if not valid in other org nrs
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public async Task<LoginOptionDto> InitialiseLoginOptions(LoginOptionEnt ent)
        {
            //Get the default org for this login option
            var org = await _orgService.GetOrg(ent.OrgNr);
            var icon = string.Empty;
            if (!string.IsNullOrEmpty(org.Icon)) icon = org.Icon;

            var orgs = new List<OrgDto>();
            var langs = new List<LangCodeDto>();
            var dto = new LoginOptionDto
            {
                UrlSuffix = ent.UrlSuffix,
                AppLoginUrl = org.AppLoginUrl,
                AppMfaUrl = org.AppMfaUrl,
                Code = org.Code,
                Icon = icon,
                OrgNr = ent.OrgNr,
                LangCode = ent.LangCode,
                LangLabelVariant = ent.LangLabelVariant,
                Orgs = orgs,
                LangCodes = langs,
                SuccessAction = ent.SuccessAction,
                Mfa = org.Mfa > GC.MfaInactive,
                RememberMe = org.IsRememberMeEnabled,
                PasswordReset = org.IsPasswordResetEnabled,
                PasswordResetCaptcha = org.Encoding.IsPasswordResetCaptchaEnabled,
                SelfRegistration = org.IsSignupEnabled,
                SelfRegistrationCaptcha = org.Encoding.IsSignupCaptchaEnabled,
                Masquerade = org.IsMasqueradeEnabled && ent.IsMasqueradeEnabled  //Must be both enabled
            };

            foreach (var part in ent.OrgNrs.Split(","))
            {
                var o = await _orgService.GetOrg(int.Parse(part));
                if (!ent.IsService && !o.IsActive) continue;
                dto.Orgs.Add(new OrgDto
                {
                    Nr = o.Nr,
                    Code = o.Code,
                    Description = o.Description
                });
            }

            dto.Orgs = dto.Orgs
                .OrderBy(o => o.Nr == dto.OrgNr ? 0 : 1)
                .ThenBy(o => o.Nr)
                .ToList();

            await ReconcileOptionsWithOrgs(dto);

            if (dto.SelfRegistrationCaptcha || dto.PasswordResetCaptcha)
                dto.RecaptchaSiteKey = AppSettings.ReCaptcha.SiteKey;

            var list = await _labelService.GetLangCodeList();
            foreach (var part in ent.LangCodes.Split(","))
            {
                var l = list.Find(l => l.Code == part);
                if (l == null || (!ent.IsService && !l.IsActive)) continue;
                dto.LangCodes.Add(new LangCodeDto
                {
                    Code = l.Code,
                    Description = l.Description,
                    Flag = l.Flag,
                });

            }
            return dto;
        }

        //Make sure the options are enabled in at least one of the org numbers
        private async Task ReconcileOptionsWithOrgs(LoginOptionDto dto)
        {
            var test = new LoginOptionDto
            {
                Mfa = true,
                RememberMe = false,
                PasswordReset = false,
                PasswordResetCaptcha = false,
                SelfRegistration = false,
                SelfRegistrationCaptcha = false,
                Masquerade = false,
            };

            foreach (var orgDto in dto.Orgs)
            {
                var org = await _orgService.GetOrg(orgDto.Nr);
                ReconcileOptionsWithOrg(test, org);
            }

            if (!test.Mfa) dto.Mfa = false;
            if (!test.RememberMe) dto.RememberMe = false;
            if (!test.PasswordReset) dto.PasswordReset = false;
            if (!test.PasswordResetCaptcha) dto.PasswordResetCaptcha = false;
            if (!test.SelfRegistration) dto.SelfRegistration = false;
            if (!test.SelfRegistrationCaptcha) dto.SelfRegistrationCaptcha = false;
            if (!test.Masquerade) dto.Masquerade = false;
        }

        private void ReconcileOptionsWithOrg(LoginOptionDto test, OrgEnt org)
        {
            if (org == null || !org.IsActive) return;

            if (org.Mfa > GC.MfaInactive) test.Mfa = true;
            if (org.IsRememberMeEnabled) test.RememberMe = true;
            if (org.IsMasqueradeEnabled) test.Masquerade = true;
            if (org.IsPasswordResetEnabled) test.PasswordReset = true;
            if (org.Encoding.IsPasswordResetCaptchaEnabled) test.PasswordResetCaptcha = true;
            if (org.IsSignupEnabled) test.SelfRegistration = true;
            if (org.Encoding.IsSignupCaptchaEnabled) test.SelfRegistrationCaptcha = true;
        }

    }
}
