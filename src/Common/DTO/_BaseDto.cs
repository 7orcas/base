
/// <summary>
/// Base class for entities that implement the usual fields
/// Contains hashcoding 
/// Created: 2025
/// [*Licence*]
/// Author: March John Stewart
/// </summary>

namespace Common.DTO
{
    public abstract class _BaseDto
    {
        public long Id { get; set; }
        public int OrgNr { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset Updated { get; set; }
        public int Version { get; set; }

        public bool IsDelete { get; set; } = false;
        public bool IsError { get; set; } = false;
        public bool IsNew() => Id < 0;

        public bool IsNewable() => IsNew() && !IsDelete;
        public bool IsDeleteable() => IsDelete && !IsNew();
        public bool IsValidatable() => !IsNew();

    }
}
