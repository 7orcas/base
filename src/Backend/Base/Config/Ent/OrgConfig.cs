
/// <summary>
/// Configurations are for organisation
/// Note this is not a database entity
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Config.Ent
{
    public class OrgConfig
    {
        public int orgNr { get; set; }
        public string LangCodeDefault { get; set; }
        public bool IsLangCodeEditable { get; set; } 
        public List<LanguageConfig> Languages { get; set; }
    }
}
