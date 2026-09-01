
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Backend.Base.User
{
    public interface UserServiceI
    {
        Task<UserEnt> GetUserById(long id);
        Task<List<UserEnt>> GetUserList(UserSearch search);
        Task<UserEnt?> UpdateUser(UserDto user);
        Task<UserDto?> NewUser(SessionEnt session);
        Task<UserDto> PopulateList(UserEnt user);
        Task<UserDto> Populate(UserEnt user);
        Task<DefinitionDto> GetDefinition(SessionEnt session);
        Task<List<ValidationDto>> ValidateUser(SessionEnt session, List<UserDto> update);
    }
}
