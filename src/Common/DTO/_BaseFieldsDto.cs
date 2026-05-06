
/// <summary>
/// Base class for entities that implement the usual fields
/// Contains hashcoding 
/// Created: 2025
/// [*Licence*]
/// Author: March John Stewart
/// </summary>

namespace Common.DTO
{
    public abstract class _BaseFieldsDto<T> : _BaseDto<T> where T : _BaseDto<T>
    {
        public long Id { get; set; }
        public int orgNr { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public DateTime Updated { get; set; }
        public bool IsActive { get; set; }

        public bool IsNew() => Id < 0;
        
    }
}
