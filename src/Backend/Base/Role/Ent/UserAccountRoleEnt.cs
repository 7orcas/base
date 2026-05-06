
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Interesction of user account - role
/// Created: April 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Role.Ent
{
    public class UserAccountRoleEnt 
    {
        public long Id { get; set; }
        public long RoleId { set; get; }
        public long UserAccountId { set; get; }
        public DateTime Updated { get; set; }
        public bool IsActive { get; set; }

        [NotMapped] public string? Code { get; set; }
        [NotMapped] public string? Description { get; set; }
        [NotMapped] public int? orgNr { get; set; }
    }
}
