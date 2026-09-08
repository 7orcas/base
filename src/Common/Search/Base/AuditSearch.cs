
namespace Common.Search.Base
{
    public class AuditSearch : _BaseSearch
    {
        public string? Username { get; set; }

        public bool IsValid() => true;

    }
}
