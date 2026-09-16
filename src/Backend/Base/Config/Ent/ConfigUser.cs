
/// <summary>
/// Configurations are for user account
/// Created: March 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Config.Ent
{
    [Obsolete("DELETE ME")]
    public class ConfigUser
    {
        public int orgNr { get; set; }
        public string LangCodeCurrent { get; set; }
        
        public List<ConfigLanguage> Languages { get; set; }
       
    }

}
