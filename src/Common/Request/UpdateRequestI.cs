
using Common.DTO;

namespace Common.Request
{
    public interface UpdateRequestI
    {
        IEnumerable<_BaseDto> Updates { get; }
    }
}
