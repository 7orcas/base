namespace Backend.Base.Config
{
    public class ValidateLanguageCode
    {

        /// <summary>
        /// Make sure the user is using a valid language code
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="orgConfig"></param>
        /// <param name="requestedLangCode"></param>
        /// <returns></returns>
        public static string Validate(LoginAccountEnt userAccount, ConfigOrg orgConfig, string? requestedLangCode)
        {
            return Validate(userAccount.IsService(), orgConfig, requestedLangCode);
        }

        /// <summary>
        /// Make sure the user is using a valid language code
        /// </summary>
        /// <param name="session"></param>
        /// <param name="orgConfig"></param>
        /// <param name="requestedLangCode"></param>
        /// <returns></returns>
        public static string Validate(SessionEnt session, ConfigOrg orgConfig, string? requestedLangCode)
        {
            return Validate(session.IsService, orgConfig, requestedLangCode);
        }

        private static string Validate(bool isService, ConfigOrg orgConfig, string? requestedLangCode)
        {
            //Is user using the default org lang code?
            if (requestedLangCode == null) requestedLangCode = orgConfig.LangCodeDefault;
            if (requestedLangCode.Equals(orgConfig.LangCodeDefault)) return requestedLangCode;

            //Is the user's lang code used by the organisation (and active if required)?
            foreach (var lang in orgConfig.Languages)
                if (requestedLangCode.Equals(lang.LangCode))
                {
                    if (isService) return requestedLangCode;
                    if (lang.IsActive()) return requestedLangCode;
                }

            return orgConfig.LangCodeDefault;
        }


    }
}
