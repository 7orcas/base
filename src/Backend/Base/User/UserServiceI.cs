
namespace Backend.Base.User
{
    public interface UserServiceI
    {
        Task<UserEnt> GetUser(long id);
        Task<List<UserEnt>> GetUserList();
        Task<UserEnt?> UpdateUser(UserDto user);
        Task<UserDto> PopulateAsList(UserEnt user);
        Task<UserDto> Populate(UserEnt user);
        Task<UserDto.UserAccountDto> Populate(UserAccountEnt account);
    }
}
