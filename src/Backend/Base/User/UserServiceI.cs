
namespace Backend.Base.User
{
    public interface UserServiceI
    {
        Task<UserEnt> GetUser(long id);
        Task<List<UserEnt>> GetUserList();

        UserDto Populate(UserEnt user);
    }
}
