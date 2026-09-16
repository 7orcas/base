
namespace Common.Search.Base
{
    public class UserSearch : _BaseSearch
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool IgnoreEffectDate { get; set; } = true;
        public string? Permission { get; set; }

        public override bool IsValid()
        {
            return true || !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Email);
        }
    }

}
