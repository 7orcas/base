
namespace Backend.Base.User
{
    public interface UserServiceI
    {
        Task<UserEnt> GetUser(long id);
        Task<List<UserEnt>> GetUserList();
        Task<UserEnt?> UpdateUser(UserDto user);
        UserDto Populate(UserEnt user);
    }
}
