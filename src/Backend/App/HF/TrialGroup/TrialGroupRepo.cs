
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;

namespace Backend.App.HF.TrialGroup
{
    /// <summary>
    /// Manage trialgroup database activities
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>Nov 2026</created>
    /// <license>**Licence**</license>
    public class TrialGroupRepo : BaseRepo, TrialGroupRepoI
    {
        private readonly AppDbContext _context;

        public TrialGroupRepo(AppDbContext context,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _context = context;
        }

        /// <summary>
        /// Return a list of trialgroups
        /// </summary>
        /// <param name="session"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<List<TrialGroupEnt>> GetList(SessionEnt session, TrialGroupSearch search)
        {
            var parameters = new DynamicParameters();
            var includeCode = !string.IsNullOrWhiteSpace(search.Code);

            var sql = "SELECT DISTINCT c.id, c.code, c.descr AS Description, c.updated, c.version, c.isactive " +
                       "FROM hf.trialGroup AS c " +
                       "WHERE c.id > 0 ";

            if (includeCode)
                sql += GetSqlAndClause(search.Code, "Code", parameters, "c.code", search);

            sql += " ORDER BY code";
            return await GetList<TrialGroupEnt>(sql, parameters);
        }

        /// <summary>
        /// Return the specific trialgroup for the passed in id
        /// </summary>
        /// <param name="trialGroupId"></param>
        /// <returns></returns>
        public async Task<TrialGroupEnt?> GetById(long trialGroupId)
        {
            try
            {
                return await _context.TrialGroups 
                    .FirstOrDefaultAsync(x => x.Id == trialGroupId);
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving trialgroup with ID {trialGroupId}", ex);
                return null;
            }
        }

        /// <summary>
        /// Return the specific trialgroup for the passed in code / orgNr
        /// </summary>
        /// <param name="trialGroupId"></param>
        /// <returns></returns>
        public async Task<TrialGroupEnt?> GetByCode(string code, int orgNr)
        {
            try
            {
                return await _context.TrialGroups
                    .FirstOrDefaultAsync(x => x.Code == code && x.OrgNr == orgNr);
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving trialgroup with Code {code} and OrgNr {orgNr}", ex);
                return null;
            }
        }

        /// <summary>
        /// Update, create or delete a TrialGroup
        /// </summary>
        /// <param name="session"></param>
        /// <param name="trialGroup"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TrialGroupEnt> Update(SessionEnt session, TrialGroupEnt trialGroup, TrialGroupDto dto)
        {
            if (dto.IsDelete)
            {
                _context.TrialGroups.Remove(trialGroup);
                await _context.SaveChangesAsync();
                return null;
            }
            
            trialGroup.Code = dto.Code;
            trialGroup.Description = dto.Description;
            trialGroup.OrgNr = dto.OrgNr;
            trialGroup.IsActive = dto.IsActive;
            VersionIncrement(trialGroup);

            trialGroup.Encode();

            if (dto.IsNew())
                _context.TrialGroups.Add(trialGroup);

            await _context.SaveChangesAsync();
            return trialGroup;
        }

    }
}
