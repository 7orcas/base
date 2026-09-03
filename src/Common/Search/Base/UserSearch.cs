
namespace Common.Search.Base
{
    public class UserSearch : _BaseSearch
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        
        public bool IsValid()
        {
            return true || !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Email);
        }
           }

}
