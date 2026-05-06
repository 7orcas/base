
/// <summary>
/// Roles have permissions
/// Roles are assigned to user accounts
/// Users can have multiple roles, the highest permission will be used
/// Created: April 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Role.Ent
{
    public class RoleEnt : BaseEntity <RoleEnt>
    {
        public RoleEnt() { }

        public List<RolePermissionEnt> RolePermissions { get; set; }

        public override void Decode() { }
        public override void Encode() { }
    }
}
