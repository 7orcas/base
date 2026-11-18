
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;

namespace Backend.CodeTemplate.Generic
{
    /// <summary>
    /// Manage codetemplate database activities
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>*Created*</created>
    /// <license>**Licence**</license>
    public class CodeTemplateRepo : BaseRepo, CodeTemplateRepoI
    {
        private readonly AppDbContext _context;

        public CodeTemplateRepo(AppDbContext context,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _context = context;
        }

        /// <summary>
        /// Return a list of codetemplates
        /// </summary>
        /// <param name="session"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<List<CodeTemplateEnt>> GetList(SessionEnt session, CodeTemplateSearch search)
        {
            var parameters = new DynamicParameters();
            var includeCode = !string.IsNullOrWhiteSpace(search.Code);

            var sql = "SELECT DISTINCT c.id, c.code, c.descr AS Description, c.updated, c.version, c.isactive " +
                       "FROM base.zcodetemplate AS c " +
                       "WHERE c.id > 0 ";

            if (includeCode)
                sql += GetSqlAndClause(search.Code, "Code", parameters, "c.code", search);

            sql += " ORDER BY code";
            return await GetList<CodeTemplateEnt>(sql, parameters);
        }

        /// <summary>
        /// Return the specific codetemplate for the passed in id
        /// </summary>
        /// <param name="codeTemplateId"></param>
        /// <returns></returns>
        public async Task<CodeTemplateEnt?> GetById(long codeTemplateId)
        {
            try
            {
                return await _context.CodeTemplates 
                    .FirstOrDefaultAsync(x => x.Id == codeTemplateId);
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving codetemplate with ID {codeTemplateId}", ex);
                return null;
            }
        }

        /// <summary>
        /// Return the specific codetemplate for the passed in code / orgNr
        /// </summary>
        /// <param name="codeTemplateId"></param>
        /// <returns></returns>
        public async Task<CodeTemplateEnt?> GetByCode(string code, int orgNr)
        {
            try
            {
                return await _context.CodeTemplates
                    .FirstOrDefaultAsync(x => x.Code == code && x.OrgNr == orgNr);
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving codetemplate with Code {code} and OrgNr {orgNr}", ex);
                return null;
            }
        }

        /// <summary>
        /// Update, create or delete a CodeTemplate
        /// </summary>
        /// <param name="session"></param>
        /// <param name="codeTemplate"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<CodeTemplateEnt> Update(SessionEnt session, CodeTemplateEnt codeTemplate, CodeTemplateDto dto)
        {
            if (dto.IsDelete)
            {
                _context.CodeTemplates.Remove(codeTemplate);
                await _context.SaveChangesAsync();
                return null;
            }
            
            codeTemplate.Code = dto.Code;
            codeTemplate.Description = dto.Description;
            codeTemplate.OrgNr = dto.OrgNr;
            codeTemplate.IsActive = dto.IsActive;
            VersionIncrement(codeTemplate);

            codeTemplate.Encode();

            if (dto.IsNew())
                _context.CodeTemplates.Add(codeTemplate);

            await _context.SaveChangesAsync();
            return codeTemplate;
        }

    }
}
