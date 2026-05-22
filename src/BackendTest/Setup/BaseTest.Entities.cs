using Moq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest.Setup
{
    public partial class BaseTest
    {

        public const int ORG_NR = 1;
        public const string LANG_CODE_EN = "en";
        public const string LANG_CODE_DE = "de";
        public const string LANG_CODE_ES = "es";
        public const string LANG_KEY_CODE = "Lang";
        public const int LANG_KEY_ID = 1;
        public const string ROLE_1 = "Role1";
        public const string ROLE_2 = "Role2";
        public const string ROLE_3 = "Role3";
        public const int ROLE_1_ID = 1;
        public const int USER_ACCOUNT_ID_1 = 1;
        public const int USER_ACCOUNT_ID_2 = 2;


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
                    orgNr = ORG_NR,
                    LangCodeCurrent = LANG_CODE_EN,
                    Languages = CreateLanguageConfigs()
            };
            return userConfig;
        }

        public async Task<OrgEnt> GetOrgEnt()
        {
            //  var org = await orgService.GetOrg(GCT.OrgNr);
            // return org;
            return null;
        }

        public LabelServiceI GetLabelService()
        {
            var service = new Mock<LabelServiceI>();

            service
                .Setup(x => x.GetLanguageLabelList(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(CreateLangLabels());

            service
                .Setup(x => x.GetLanguageKey(It.IsAny<string>()))
                .ReturnsAsync(new LangKey
                {
                    Id = 1
                });

            service
                .Setup(x => x.GetRelatedLabels(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(CreateLangLabels());

            service
                .Setup(x => x.GetLanguageLabelDic(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(CreateLanguageDictionary());

            return service.Object;
        }

        public static List<LangLabel> CreateLangLabels()
        {
            return new List<LangLabel>
            {
                new LangLabel { Id = 1, LangKeyId = LANG_KEY_ID, LangCode = LANG_CODE_EN, LangKeyCode = LANG_KEY_CODE, Code = "Language" },
                new LangLabel { Id = 2, LangKeyId = LANG_KEY_ID, LangCode = LANG_CODE_DE, LangKeyCode = LANG_KEY_CODE, Code = "Sprache" },
                new LangLabel { Id = 3, LangKeyId = LANG_KEY_ID, LangCode = LANG_CODE_ES, LangKeyCode = LANG_KEY_CODE, Code = "Idiom" }
            };
        }

        public static List<LanguageConfig> CreateLanguageConfigs()
        {
            return new List<LanguageConfig>
            {
                new LanguageConfig { LangCode = LANG_CODE_EN, IsReadonly = false, IsEditable = true },
                new LanguageConfig { LangCode = LANG_CODE_DE, IsReadonly = true, IsEditable = false },
                new LanguageConfig { LangCode = LANG_CODE_ES, IsReadonly = false, IsEditable = false }
            };
        }

        public static Dictionary<string, LangLabel> CreateLanguageDictionary()
        {
            var dic = new Dictionary<string, LangLabel>();
            var list = CreateLangLabels();

            foreach (var label in list)
                dic.Add(label.LangCode, label);
            return dic;
        }

        public RoleServiceI GetRoleService()
        {
            var service = new Mock<RoleServiceI>();
            
            service
                .Setup(x => x.GetRoles(It.IsAny<SessionEnt>()))
                .ReturnsAsync(GetRoles());
            service
                .Setup(x => x.GetUserRoles(It.IsAny<SessionEnt>()))
                .ReturnsAsync(GetUserRoles());
            service
                .Setup(x => x.GetRole(It.IsAny<long>()))
                .ReturnsAsync(GetRoles()[0]);

            return service.Object;
        }

        public static List<RoleEnt> GetRoles()
        {
            var list = new List<RoleEnt>
            {
                new RoleEnt { Id = 1, Code = ROLE_1 },
                new RoleEnt { Id = 2, Code = ROLE_2 },
                new RoleEnt { Id = 3, Code = ROLE_3 }
            };
            foreach (var role in list)
            {
                role.RolePermissions = new List<RolePermissionEnt>
                {
                    new RolePermissionEnt { Id = 1, PermissionNr = 1, RoleId = role.Id },
                    new RolePermissionEnt { Id = 2, PermissionNr = 2, RoleId = role.Id },
                    new RolePermissionEnt { Id = 3, PermissionNr = 3, RoleId = role.Id }
                };
            }
            return list;
        }

        public static List<UserAccountRoleEnt> GetUserRoles()
        {
            return new List<UserAccountRoleEnt>
            {
                new UserAccountRoleEnt { Id = 1, RoleId = 1, Code = ROLE_1 , UserAccountId = USER_ACCOUNT_ID_1 , orgNr = ORG_NR},
                new UserAccountRoleEnt { Id = 2, RoleId = 2, Code = ROLE_2 , UserAccountId = USER_ACCOUNT_ID_1 , orgNr = ORG_NR },
                new UserAccountRoleEnt { Id = 3, RoleId = 3, Code = ROLE_3 , UserAccountId = USER_ACCOUNT_ID_1 , orgNr = ORG_NR }
            };
        }

        public PermissionInitialiseServiceI GetPermissionInitialiseService()
        {
            var service = new Mock<PermissionInitialiseServiceI>();
            var serviceO = service.Object;
            serviceO.InitialisePermissions();
            service
                .Setup(x => x.GetPermissions())
                .Returns(InitialisePermissions());
            return serviceO;
        }
        public Dictionary<int, PermissionEnt> InitialisePermissions()
        {
            var dic = new Dictionary<int, PermissionEnt>(); //permission nr, entity

            for (int i = 0; i < GC.Permissions.Length; i += 2)
            {
                var nr = (int)GC.Permissions[i];
                var langKey = (string)GC.Permissions[i + 1];
                dic.Add(nr, new PermissionEnt
                {
                    Nr = nr,
                    LangKey = langKey,
                });
            }
            return dic;
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
