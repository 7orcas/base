
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;

namespace Backend.App.HF.TrailGroup
{
    /// <summary>
    /// Manage trailgroup database activities
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>Nov 2026</created>
    /// <license>**Licence**</license>
    public class TrailGroupRepo : BaseRepo, TrailGroupRepoI
    {
        private readonly AppDbContext _context;

        public TrailGroupRepo(AppDbContext context,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _context = context;
        }

        /// <summary>
        /// Return a list of trailgroups
        /// </summary>
        /// <param name="session"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<List<TrailGroupEnt>> GetList(SessionEnt session, TrailGroupSearch search)
        {
            var parameters = new DynamicParameters();
            var includeCode = !string.IsNullOrWhiteSpace(search.Code);

            var sql = "SELECT DISTINCT c.id, c.code, c.descr AS Description, c.updated, c.version, c.isactive " +
                       "FROM hf.trailGroup AS c " +
                       "WHERE c.id > 0 ";

            if (includeCode)
                sql += GetSqlAndClause(search.Code, "Code", parameters, "c.code", search);

            sql += " ORDER BY code";
            return await GetList<TrailGroupEnt>(sql, parameters);
        }

        /// <summary>
        /// Return the specific trailgroup for the passed in id
        /// </summary>
        /// <param name="trailGroupId"></param>
        /// <returns></returns>
        public async Task<TrailGroupEnt?> GetById(long trailGroupId)
        {
            try
            {
                return await _context.TrailGroups 
                    .FirstOrDefaultAsync(x => x.Id == trailGroupId);
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving trailgroup with ID {trailGroupId}", ex);
                return null;
            }
        }

        /// <summary>
        /// Return the specific trailgroup for the passed in code / orgNr
        /// </summary>
        /// <param name="trailGroupId"></param>
        /// <returns></returns>
        public async Task<TrailGroupEnt?> GetByCode(string code, int orgNr)
        {
            try
            {
                return await _context.TrailGroups
                    .FirstOrDefaultAsync(x => x.Code == code && x.OrgNr == orgNr);
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving trailgroup with Code {code} and OrgNr {orgNr}", ex);
                return null;
            }
        }

        /// <summary>
        /// Update, create or delete a TrailGroup
        /// </summary>
        /// <param name="session"></param>
        /// <param name="trailGroup"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TrailGroupEnt> Update(SessionEnt session, TrailGroupEnt trailGroup, TrailGroupDto dto)
        {
            if (dto.IsDelete)
            {
                _context.TrailGroups.Remove(trailGroup);
                await _context.SaveChangesAsync();
                return null;
            }
            
            trailGroup.Code = dto.Code;
            trailGroup.Description = dto.Description;
            trailGroup.OrgNr = dto.OrgNr;
            trailGroup.IsActive = dto.IsActive;
            VersionIncrement(trailGroup);

            trailGroup.Encode();

            if (dto.IsNew())
                _context.TrailGroups.Add(trailGroup);

            await _context.SaveChangesAsync();
            return trailGroup;
        }

    }
}
