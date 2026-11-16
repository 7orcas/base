
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;

namespace Backend.App.HF.Trail
{
    /// <summary>
    /// Manage trail database activities
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>Nov 2026</created>
    /// <license>**Licence**</license>
    public class TrailRepo : BaseRepo, TrailRepoI
    {
        private readonly AppDbContext _context;

        public TrailRepo(AppDbContext context,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _context = context;
        }

        /// <summary>
        /// Return a list of trails
        /// </summary>
        /// <param name="session"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<List<TrailEnt>> GetList(SessionEnt session, TrailSearch search)
        {
            var parameters = new DynamicParameters();
            var includeCode = !string.IsNullOrWhiteSpace(search.Code);

            var sql = "SELECT DISTINCT c.id, c.code, c.descr AS Description, c.updated, c.version, c.isactive " +
                       "FROM hf.trail AS c " +
                       "WHERE c.id > 0 ";

            if (includeCode)
                sql += GetSqlAndClause(search.Code, "Code", parameters, "c.code", search);

            sql += " ORDER BY code";
            return await GetList<TrailEnt>(sql, parameters);
        }

        /// <summary>
        /// Return the specific trail for the passed in id
        /// </summary>
        /// <param name="trailId"></param>
        /// <returns></returns>
        public async Task<TrailEnt?> GetById(long trailId)
        {
            try
            {
                return await _context.Trails 
                    .FirstOrDefaultAsync(x => x.Id == trailId);
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving trail with ID {trailId}", ex);
                return null;
            }
        }

        /// <summary>
        /// Return the specific trail for the passed in code / orgNr
        /// </summary>
        /// <param name="trailId"></param>
        /// <returns></returns>
        public async Task<TrailEnt?> GetByCode(string code, int orgNr)
        {
            try
            {
                return await _context.Trails
                    .FirstOrDefaultAsync(x => x.Code == code && x.OrgNr == orgNr);
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving trail with Code {code} and OrgNr {orgNr}", ex);
                return null;
            }
        }

        /// <summary>
        /// Update, create or delete a Trail
        /// </summary>
        /// <param name="session"></param>
        /// <param name="trail"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TrailEnt> Update(SessionEnt session, TrailEnt trail, TrailDto dto)
        {
            if (dto.IsDelete)
            {
                _context.Trails.Remove(trail);
                await _context.SaveChangesAsync();
                return null;
            }
            
            trail.Code = dto.Code;
            trail.Description = dto.Description;
            trail.OrgNr = dto.OrgNr;
            trail.IsActive = dto.IsActive;
            VersionIncrement(trail);

            trail.Encode();

            if (dto.IsNew())
                _context.Trails.Add(trail);

            await _context.SaveChangesAsync();
            return trail;
        }

    }
}
