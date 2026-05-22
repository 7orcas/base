using System.Net.NetworkInformation;
using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest.Setup
{
    public partial class BaseTest
    {

        public static SessionEnt CreateSessionEnt()
        {
            var session = new SessionEnt
            {
                Org = new OrgEnt { LangLabelVariant = 0 },
                UserConfig = CreateUserConfig()
            };

            return session;
        }

        public static UserConfig CreateUserConfig()
        {
            var userConfig = new UserConfig
                {
                    orgNr = GCT.orgNr,
                    LangCodeCurrent = GCT.UserLangCode,
                    Languages = CreateLanguageConfigs()
            };
            return userConfig;
        }

        
        public static List<LangLabel> CreateLangLabels()
        {
            return new List<LangLabel>
            {
                new LangLabel { Id = 1, LangKeyId = 1, LangCode = "en", LangKeyCode = "Lang", Code = "Language" },
                new LangLabel { Id = 2, LangKeyId = 1, LangCode = "de", LangKeyCode = "Lang", Code = "Sprache" },
                new LangLabel { Id = 3, LangKeyId = 1, LangCode = "es", LangKeyCode = "Lang", Code = "Idiom" }
            };
        }


        public static List<LanguageConfig> CreateLanguageConfigs()
        {
            return new List<LanguageConfig>
            {
                new LanguageConfig { LangCode = "en", IsReadonly = false, IsEditable = true },
                new LanguageConfig { LangCode = "de", IsReadonly = true, IsEditable = false },
                new LanguageConfig { LangCode = "es", IsReadonly = false, IsEditable = false }
            };
        }



        public async Task<OrgEnt> GetOrgEnt()
        {
            //  var org = await orgService.GetOrg(GCT.OrgNr);
            // return org;
            return null;
        }


        //public async Task<UserEnt> GetUserEnt()
        //{
        //    var result = await loginservice.GetLogin(GCT.UserName, GCT.OrgNr);
        //    var org = await orgService.GetOrg(GCT.OrgNr);
        //    var user = await loginservice.InitialiseLogin(result.login, org, GC.AppClient);
        //    return user;
        //}

    }
}
