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

namespace Backend.App.HF.TrailGroup
{
    /// <summary>
    /// Manage loading and processing trailgroups
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>Nov 2026</created>
    /// <license>**Licence**</license>
    public class TrailGroupService: BaseService, TrailGroupServiceI
    {
        private readonly TrailGroupRepoI _trailGroupRepo;


        public TrailGroupService(IServiceProvider serviceProvider, 
            TrailGroupRepoI trailGroupRepo) 
            : base(serviceProvider) 
        { 
            _trailGroupRepo = trailGroupRepo;
        }


        /// <summary>
        /// Return a list of trailgroups
        /// </summary>
        /// <param name="session"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<List<TrailGroupEnt>> GetTrailGroupList(SessionEnt session, TrailGroupSearch search)
        {
            return await _trailGroupRepo.GetList(session, search);
        }

        /// <summary>
        /// Populate the dto's for a list of trailgroups
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public List<TrailGroupDto> Populate(SessionEnt session, List<TrailGroupEnt> entities)
        {
            var list = new List<TrailGroupDto>();
            foreach (var entity in entities)
                list.Add(Populate(session, entity));

            return list;
        }


        /// <summary>
        /// Return the specific trailgroup for the passed in id
        /// </summary>
        /// <param name="trailGroupId"></param>
        /// <returns></returns>
        public async Task<TrailGroupEnt> GetTrailGroupById(long trailGroupId)
        {
            return await _trailGroupRepo.GetById(trailGroupId);
        }

        /// <summary>
        /// Populate a dto for a trailgroup
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TrailGroupDto Populate(SessionEnt session, TrailGroupEnt entity)
        {
            return new TrailGroupDto
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
        /// Get trailgroup field definitions
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<DefinitionDto> GetDefinition(SessionEnt session)
        {
            var validator = new TrailGroupVal(session, _trailGroupRepo);
            return validator.GetDefinition();
        }

        /// <summary>
        /// Validate a list of trailgroup dtos
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public async Task<List<ValidationDto>> ValidateTrailGroups(SessionEnt session, List<TrailGroupDto> update)
        {
            var validator = new TrailGroupVal(session, _trailGroupRepo);
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
        /// Return the specific trailgroup for the passed in dto, or create a new one if it doesn't exist
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TrailGroupEnt?> GetTrailGroupOrCreate(SessionEnt session, TrailGroupDto dto)
        {
            //No action required
            if (dto.IsNew() && dto.IsDelete)
                return null;

            if (dto.IsNew())
                return new TrailGroupEnt { 
                    OrgNr = session.OrgNr,
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                };


            return await _trailGroupRepo.GetById(dto.Id);
        }

        /// <summary>
        /// Update, create and delete a TrailGroup
        /// </summary>
        /// <param name="session"></param>
        /// <param name="trailGroup"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TrailGroupEnt> UpdateTrailGroup(SessionEnt session, TrailGroupEnt trailGroup, TrailGroupDto dto)
        {
            return await _trailGroupRepo.Update(session, trailGroup, dto);
        }

        /// <summary>
        /// Get a new TrailGroup entity with default values
        /// Note it is not saved to the database yet, it is just a template for creating a new TrailGroup dto
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<TrailGroupDto?> NewTrailGroup(SessionEnt session)
        {
            var ent = new TrailGroupEnt()
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
