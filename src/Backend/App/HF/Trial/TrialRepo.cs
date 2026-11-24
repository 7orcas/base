
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;

namespace Backend.App.HF.Trial
{
    /// <summary>
    /// Manage trial database activities
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>Nov 2026</created>
    /// <license>**Licence**</license>
    public class TrialRepo : BaseRepo, TrialRepoI
    {
        private readonly AppDbContext _context;

        public TrialRepo(AppDbContext context,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _context = context;
        }

        /// <summary>
        /// Return a list of trials
        /// </summary>
        /// <param name="session"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<List<TrialEnt>> GetList(SessionEnt session, TrialSearch search)
        {
            var parameters = new DynamicParameters();
            var includeCode = !string.IsNullOrWhiteSpace(search.Code);

            var sql = "SELECT DISTINCT c.id, c.status, c.code, c.descr AS Description, c.updated, c.version, c.isactive " +
                       "FROM hf.trial AS c " +
                       "WHERE c.id > 0 ";

            if (includeCode)
                sql += GetSqlAndClause(search.Code, "Code", parameters, "c.code", search);

            sql += " ORDER BY code";
            return await GetList<TrialEnt>(sql, parameters);
        }

        /// <summary>
        /// Return the specific trial for the passed in id
        /// </summary>
        /// <param name="trialId"></param>
        /// <returns></returns>
        public async Task<TrialEnt?> GetById(long trialId)
        {
            try
            {
                return await _context.Trials 
                    .FirstOrDefaultAsync(x => x.Id == trialId);
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving trial with ID {trialId}", ex);
                return null;
            }
        }

        /// <summary>
        /// Return the specific trial for the passed in code / orgNr
        /// </summary>
        /// <param name="trialId"></param>
        /// <returns></returns>
        public async Task<TrialEnt?> GetByCode(string code, int orgNr)
        {
            try
            {
                return await _context.Trials
                    .FirstOrDefaultAsync(x => x.Code == code && x.OrgNr == orgNr);
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving trial with Code {code} and OrgNr {orgNr}", ex);
                return null;
            }
        }

        /// <summary>
        /// Update, create or delete a Trial
        /// </summary>
        /// <param name="session"></param>
        /// <param name="trial"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TrialEnt> Update(SessionEnt session, TrialEnt trial, TrialDto dto)
        {
            if (dto.IsDelete)
            {
                _context.Trials.Remove(trial);
                await _context.SaveChangesAsync();
                return null;
            }
            
            trial.Code = dto.Code;
            trial.Description = dto.Description;
            trial.OrgNr = dto.OrgNr;
            trial.IsActive = dto.IsActive;
            trial.Status = dto.Status;
            VersionIncrement(trial);

            trial.Encode();

            if (dto.IsNew())
                _context.Trials.Add(trial);

            await _context.SaveChangesAsync();
            return trial;
        }

        /// <summary>
        /// Update, create or delete a Trial
        /// </summary>
        /// <param name="session"></param>
        /// <param name="trial"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task UpdateStatus(SessionEnt session, long trialId, int status)
        {
            var trial = await GetById(trialId);
            if (trial == null)
            {
                _log.Error($"Trial not found with ID {trialId}, update status: {status}"); 
                throw new InvalidOperationException(session.GetLabel("TrialEr1", "Trial not found"));
            }

            trial.Status = status;
            VersionIncrement(trial);
            await _context.SaveChangesAsync();
        }

    }
}
