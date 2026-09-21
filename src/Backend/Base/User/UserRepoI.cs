namespace Backend.Base.User
{
    public interface UserRepoI
    {
        Task<List<UserEnt>> GetList(UserSearch search, int orgNr);
        Task<UserEnt> Create(UserEnt User);
        Task<UserEnt?> GetById(long id);
        Task<UserEnt?> Update(UserEnt User, UserDto dto);
        Task<VersionInfo?> GetVersion(long id);
    }
}
