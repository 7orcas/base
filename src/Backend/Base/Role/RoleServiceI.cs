
namespace Backend.Base.Role
{
    public interface RoleServiceI
    {
        Task<List<RoleUserAccountEnt>> GetUserRoles(int orgNr, long userAccountId);
        Task<List<RoleUserAccountEnt>> GetUserRoles(SessionEnt session);
        Task<List<RoleEnt>> GetRoles(int orgNr);
        Task<List<RoleEnt>> GetRoles(SessionEnt session);
        Task<RoleEnt> GetRole(long id);
        Task SaveRoles(List<RoleEnt> list, List<RoleEnt> listDel, SessionEnt session);
    }
}
