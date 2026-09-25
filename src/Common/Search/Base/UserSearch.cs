
namespace Common.Search.Base
{
    public class UserSearch : _BaseSearch
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool IncludeEffectDate { get; set; } = false;
        public string? Permission { get; set; }

        public override bool IsValid()
        {
            return true || !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Email);
        }
    }

}
