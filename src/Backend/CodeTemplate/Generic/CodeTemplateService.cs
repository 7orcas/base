using Common.Validator;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Data;
using System.Text;
using System.Transactions;
using GC = Backend.GlobalConstants;

namespace Backend.CodeTemplate.Generic
{
    /// <summary>
    /// Manage loading and processing codetemplates
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>*Created*</created>
    /// <license>**Licence**</license>
    public class CodeTemplateService: BaseService, CodeTemplateServiceI
    {
        private readonly CodeTemplateRepoI _codeTemplateRepo;


        public CodeTemplateService(IServiceProvider serviceProvider, 
            CodeTemplateRepoI codeTemplateRepo) 
            : base(serviceProvider) 
        { 
            _codeTemplateRepo = codeTemplateRepo;
        }


        /// <summary>
        /// Return a list of codetemplates
        /// </summary>
        /// <param name="session"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<List<CodeTemplateEnt>> GetCodeTemplateList(SessionEnt session, CodeTemplateSearch search)
        {
            return await _codeTemplateRepo.GetList(session, search);
        }

        /// <summary>
        /// Populate the dto's for a list of codetemplates
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public List<CodeTemplateDto> Populate(SessionEnt session, List<CodeTemplateEnt> entities)
        {
            var list = new List<CodeTemplateDto>();
            foreach (var entity in entities)
                list.Add(Populate(session, entity));

            return list;
        }


        /// <summary>
        /// Return the specific codetemplate for the passed in id
        /// </summary>
        /// <param name="codeTemplateId"></param>
        /// <returns></returns>
        public async Task<CodeTemplateEnt> GetCodeTemplateById(long codeTemplateId)
        {
            return await _codeTemplateRepo.GetById(codeTemplateId);
        }

        /// <summary>
        /// Populate a dto for a codetemplate
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CodeTemplateDto Populate(SessionEnt session, CodeTemplateEnt entity)
        {
            return new CodeTemplateDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Description = entity.Description,
                IsActive = entity.IsActive,
                Version = entity.Version,
                Updated = entity.Updated,
            };
        }

        /// <summary>
        /// Get codetemplate field definitions
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<DefinitionDto> GetDefinition(SessionEnt session)
        {
            var validator = new CodeTemplateVal(session, _codeTemplateRepo);
            return validator.GetDefinition();
        }

        /// <summary>
        /// Validate a list of codetemplate dtos
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public async Task<List<ValidationDto>> ValidateCodeTemplates(SessionEnt session, List<CodeTemplateDto> update)
        {
            var validator = new CodeTemplateVal(session, _codeTemplateRepo);
            var vals = new List<ValidationDto>();

            foreach (var dto in update)
            {
                if (dto.IsDelete) continue;

                dto.OrgNr = session.OrgNr;

                var val = await validator.Validate(dto);
                if (val != null)
                    vals.Add(val);
            }
            return vals;
        }

        /// <summary>
        /// Return the specific codetemplate for the passed in dto, or create a new one if it doesn't exist
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<CodeTemplateEnt?> GetCodeTemplateOrCreate(SessionEnt session, CodeTemplateDto dto)
        {
            //No action required
            if (dto.IsNew() && dto.IsDelete)
                return null;

            if (dto.IsNew())
                return new CodeTemplateEnt { 
                    OrgNr = session.OrgNr,
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                };


            return await _codeTemplateRepo.GetById(dto.Id);
        }

        /// <summary>
        /// Update, create and delete a CodeTemplate
        /// </summary>
        /// <param name="session"></param>
        /// <param name="codeTemplate"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<CodeTemplateEnt> UpdateCodeTemplate(SessionEnt session, CodeTemplateEnt codeTemplate, CodeTemplateDto dto)
        {
            return await _codeTemplateRepo.Update(session, codeTemplate, dto);
        }

        /// <summary>
        /// Get a new CodeTemplate entity with default values
        /// Note it is not saved to the database yet, it is just a template for creating a new CodeTemplate dto
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<CodeTemplateDto?> NewCodeTemplate(SessionEnt session)
        {
            var ent = new CodeTemplateEnt()
            {
                Id = GetTempId(),
                OrgNr = session.Org.Nr,
                Code = "",
                Description = "",
                IsActive = true,
                Version = GC.NewRecordVersion,
            };

            return Populate(session, ent);
        }
    }
}
