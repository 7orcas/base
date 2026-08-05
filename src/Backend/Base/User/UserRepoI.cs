namespace Backend.Base.User
{
    public interface UserRepoI
    {
        Task<UserEnt> Create(UserEnt User);
        Task<UserEnt?> GetById(long id);
        Task<UserEnt?> Update(UserDto user);
    }
}
