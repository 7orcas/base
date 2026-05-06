
using Backend.Base.Permission.Ent;

namespace Backend.Base.Permission
{
    public interface PermissionServiceI
    {
        PermissionEnt GetPermissionEnt(int permNr);
        Task<List<PermissionCrudEnt>> LoadEffectivePermissions(SessionEnt session);
        Task<List<PermissionCrudEnt>> LoadEffectivePermissionsInt(long userId, long orgNr);
        Task<List<RolePermissionCrudEnt>> GetPermissions(SessionEnt session);

        bool IsAuthorizedToAccessEndPoint(SessionEnt session, PermissionAtt perm, CrudAtt crud);
    }
}
