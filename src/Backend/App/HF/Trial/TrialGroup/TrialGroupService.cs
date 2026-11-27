using Backend.App.HF.Trial.TrialGroup.Ent;
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

namespace Backend.App.HF.Trial.TrialGroup
{
    /// <summary>
    /// Manage loading and processing trialgroups
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>Nov 2026</created>
    /// <license>**Licence**</license>
    public class TrialGroupService: BaseService, TrialGroupServiceI
    {
        private readonly TrialGroupRepoI _trialGroupRepo;


        public TrialGroupService(IServiceProvider serviceProvider, 
            TrialGroupRepoI trialGroupRepo) 
            : base(serviceProvider) 
        { 
            _trialGroupRepo = trialGroupRepo;
        }


        /// <summary>
        /// Return a list of trialgroups
        /// </summary>
        /// <param name="session"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<List<TrialGroupEnt>> GetTrialGroupList(SessionEnt session, TrialGroupSearch search)
        {
            return await _trialGroupRepo.GetList(session, search);
        }

        /// <summary>
        /// Populate the dto's for a list of trialgroups
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public List<TrialGroupDto> Populate(SessionEnt session, List<TrialGroupEnt> entities)
        {
            var list = new List<TrialGroupDto>();
            foreach (var entity in entities)
                list.Add(Populate(session, entity));

            return list;
        }

        /// <summary>
        /// Return a list of trialgroup members
        /// </summary>
        /// <param name="session"></param>
        /// <param name="trialGroupId"></param>
        /// <returns></returns>
        public async Task<List<TrialGroupMemberEnt>> GetTrialGroupMembers(SessionEnt session, long trialGroupId)
        {
            return await _trialGroupRepo.GetMembers(session, trialGroupId);
        }

        /// <summary>
        /// Populate the dto's for a list of trialgroup memebers
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public List<TrialGroupMemberDto> Populate(SessionEnt session, List<TrialGroupMemberEnt> entities)
        {
            var list = new List<TrialGroupMemberDto>();
            foreach (var entity in entities)
                list.Add(Populate(session, entity));

            return list;
        }

        /// <summary>
        /// Populate a dto for a trialgroup member
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TrialGroupMemberDto Populate(SessionEnt session, TrialGroupMemberEnt entity)
        {
            return new TrialGroupMemberDto
            {
                Id = entity.Id,
                TrialGroupId = entity.TrialGroupId,
                UserName = entity.UserName,
                Email = entity.Email,
                IsAdmin = entity.IsAdmin
            };
        }

        /// <summary>
        /// Return the specific trialgroup for the passed in id
        /// </summary>
        /// <param name="trialGroupId"></param>
        /// <returns></returns>
        public async Task<TrialGroupEnt> GetTrialGroupById(long trialGroupId)
        {
            return await _trialGroupRepo.GetById(trialGroupId);
        }

        /// <summary>
        /// Populate a dto for a trialgroup
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TrialGroupDto Populate(SessionEnt session, TrialGroupEnt entity)
        {
            return new TrialGroupDto
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
        /// Get trialgroup field definitions
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<DefinitionDto> GetDefinition(SessionEnt session)
        {
            var validator = new TrialGroupVal(session, _trialGroupRepo);
            return validator.GetDefinition();
        }

        /// <summary>
        /// Validate a list of trialgroup dtos
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public async Task<List<ValidationDto>> ValidateTrialGroups(SessionEnt session, List<TrialGroupDto> update)
        {
            var validator = new TrialGroupVal(session, _trialGroupRepo);
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
        /// Return the specific trialgroup for the passed in dto, or create a new one if it doesn't exist
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TrialGroupEnt?> GetTrialGroupOrCreate(SessionEnt session, TrialGroupDto dto)
        {
            //No action required
            if (dto.IsNew() && dto.IsDelete)
                return null;

            if (dto.IsNew())
                return new TrialGroupEnt { 
                    OrgNr = session.OrgNr,
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                };


            return await _trialGroupRepo.GetById(dto.Id);
        }

        /// <summary>
        /// Update, create and delete a TrialGroup
        /// </summary>
        /// <param name="session"></param>
        /// <param name="trialGroup"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TrialGroupEnt> UpdateTrialGroup(SessionEnt session, TrialGroupEnt trialGroup, TrialGroupDto dto)
        {
            return await _trialGroupRepo.Update(session, trialGroup, dto);
        }

        /// <summary>
        /// Get a new TrialGroup entity with default values
        /// Note it is not saved to the database yet, it is just a template for creating a new TrialGroup dto
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<TrialGroupDto?> NewTrialGroup(SessionEnt session)
        {
            var ent = new TrialGroupEnt()
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
