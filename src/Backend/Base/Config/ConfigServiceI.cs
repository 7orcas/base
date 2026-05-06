using Backend.Base.Config.Ent;

namespace Backend.Base.Config
{
    public interface ConfigServiceI
    {
        UserConfig CreateUserConfig(UserAccountEnt userAccount, OrgEnt org, string? langCode);
    }
}
