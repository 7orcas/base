
/// <summary>
/// Configurations are for user account
/// Created: March 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Config.Ent
{
    public class UserConfig
    {
        public int orgNr { get; set; }
        public string LangCodeCurrent { get; set; }
        
        public List<LanguageConfig> Languages { get; set; }
       
    }

}
