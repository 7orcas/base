
/// <summary>
/// Configurations are languages used by organisation and users
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Config.Ent
{
    public class LanguageConfig
    {
        public string LangCode { get; set; }
        public bool IsReadonly { get; set; } = false;
        public bool IsEditable { get; set; } = false;

        public bool IsActive() => IsReadonly || IsEditable;
        
    }
}
