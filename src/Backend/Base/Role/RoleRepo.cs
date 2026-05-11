
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;

namespace Backend.Base.Role
{
    /// <summary>
    /// Manage role database activities
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>May 11, 2026</created>
    /// <license>**Licence**</license>
    public class RoleRepo : RoleRepoI
    {


        private readonly AppDbContext _context;

        public RoleRepo(AppDbContext context)
        {
            _context = context;
        }


        public async Task<RoleEnt> Create(RoleEnt role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }



    }
}
