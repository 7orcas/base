
/// <summary>
/// Initialisation of persmissions at program start
/// Created: March 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>


namespace Backend.Base.Permission
{
    public interface PermissionInitialiseServiceI
    {
        void InitialisePermissions();
        Dictionary<int, PermissionEnt> GetPermissions();
    }
}
