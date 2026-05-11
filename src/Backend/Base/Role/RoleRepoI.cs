namespace Backend.Base.Role
{
    public interface RoleRepoI
    {
        Task<RoleEnt> Create(RoleEnt role);
    }
}
