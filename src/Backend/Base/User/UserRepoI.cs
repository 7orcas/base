namespace Backend.Base.User
{
    public interface UserRepoI
    {
        Task<List<UserEnt>> GetList(UserSearch search);
        Task<UserEnt> Create(UserEnt User);
        Task<UserEnt?> GetById(long id);
        Task<UserEnt?> Update(UserDto user);
        Task<VersionInfo?> GetVersion(long id);
    }
}
