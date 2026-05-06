
/// <summary>
/// Base entity for application entities
/// Created: March 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Entity
{
    public abstract class BaseEntity<E> : BaseEncode
    {
        public long Id { get; set; }
        public int OrgNr { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public DateTime Updated { get; set; }
        public bool IsActive { get; set; }

        public BaseEntity() { }

        public bool IsNew() => Id < 0;
    }        
}
