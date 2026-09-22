
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Backend.Base.User
{
    public interface UserServiceI
    {
        Task<UserEnt> GetUserById(long id);
        Task<UserEnt?> GetUserOrCreate(UserDto dto);
        Task<List<UserEnt>> GetUserList(SessionEnt session, UserSearch search);
        Task<UserEnt?> UpdateUser(UserEnt user, UserDto dto);
        Task<UserDto?> NewUser(SessionEnt session);
        Task<UserDto> PopulateList(SessionEnt session, UserEnt user);
        Task<UserDto> PopulateDto(SessionEnt session, UserEnt user);
        Task<DefinitionDto> GetDefinition(SessionEnt session);
        Task<List<ValidationDto>> ValidateUser(SessionEnt session, List<UserDto> update);
    }
}
