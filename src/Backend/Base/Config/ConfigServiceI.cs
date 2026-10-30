using Backend.Base.Config.Ent;

namespace Backend.Base.Config
{
    public interface ConfigServiceI
    {
        ConfigUser CreateUserConfig(LoginAccountEnt userAccount, OrgEnt org, string? langCode);
        Task<DefinitionDto> GetDefinition(SessionEnt session);
        Task<ConfigDto> PopulateDto(SessionEnt session);
        string GetUserCacheKey(int orgNr);
    }
}
