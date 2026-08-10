
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;

namespace Backend.Base.User
{
    /// <summary>
    /// Manage User database activities
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>May 11, 2026</created>
    /// <license>**Licence**</license>
    public class UserRepo : BaseRepo, UserRepoI
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _context = context;
        }

        public async Task<List<UserEnt>> GetList()
        {
            var list = new List<UserEnt>();
            await Sql.Run(
                    "SELECT * FROM base.zzz ORDER BY zzz ",
                    r => {
                        var user = new UserEnt();
                        user.Id = GetId(r);
                        user.Username = GetString(r, "xxx");
                        user.IsActive = IsActive(r);
                        user.OrgNrDefault = GetInt(r, "orgnrdefault");
                        list.Add(user);
                    }
                );
            return list;
        }


        public async Task<UserEnt> Create(UserEnt User)
        {
            _context.Users.Add(User);
            await _context.SaveChangesAsync();
            return User;
        }

        public async Task<UserEnt?> GetById(long id)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Accounts)
                    .FirstOrDefaultAsync(x => x.Id == id);

            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving user with ID {id}", ex);
                return null;
            }
        }

        public async Task<UserEnt?> Update(UserDto user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);

            if (existingUser == null)
            {
                return null;
            }
            existingUser.Encode();
            existingUser.Username = user.Username;
            existingUser.OrgNrDefault = user.OrgNr;
            existingUser.Attempts = user.Attempts;

            existingUser.IsActive = user.IsActive;
            existingUser.Version = user.Version + 1;
            existingUser.Updated = DateTimeOffset.UtcNow;

            //Update all fields
            //_context.Entry(existingUser).CurrentValues.SetValues(user);
            // Update any other properties you want to allow changes to

            await _context.SaveChangesAsync();

            return existingUser;
        }
    }
}
