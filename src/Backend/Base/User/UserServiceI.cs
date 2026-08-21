
namespace Backend.Base.User
{
    public interface UserServiceI
    {
        Task<UserEnt> GetUser(long id);
        Task<List<UserEnt>> GetUserList(UserSearch search);
        Task<UserEnt?> UpdateUser(UserDto user);
        Task<UserDto> PopulateList(UserEnt user);
        Task<UserDto> Populate(UserEnt user);
    }
}
