
using Common.DTO;

namespace Common.Request
{
    public class UpdateRequest<T> : UpdateRequestI where T : IEnumerable<_BaseDto>
    {
        public T? Updates { get; set; }
        IEnumerable<_BaseDto> UpdateRequestI.Updates => Updates ?? Enumerable.Empty<_BaseDto>();
        public bool OverrideWarning { get; set; }
    }
}
