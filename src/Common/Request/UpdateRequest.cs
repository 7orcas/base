
namespace Common.Request
{
    public class UpdateRequest<T>
    {
        public T? Updates { get; set; }
        public bool OverrideWarning { get; set; }
    }
}
