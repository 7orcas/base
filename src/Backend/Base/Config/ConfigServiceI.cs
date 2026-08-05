using Backend.Base.Config.Ent;

namespace Backend.Base.Config
{
    public interface ConfigServiceI
    {
        ConfigUser CreateUserConfig(LoginAccountEnt userAccount, OrgEnt org, string? langCode);
    }
}
