using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Emit;

namespace Backend.App.HF.TrailGroup.Ent
{
    public class TrailGroupVal : BaseValidation <TrailGroupDto>
    {

        private TrailGroupRepoI _trailGroupRepo;

        public TrailGroupVal(SessionEnt session, 
            TrailGroupRepoI trailGroupRepo)
            : base(session)
        {
            _trailGroupRepo = trailGroupRepo;
        }

        protected override void Configure()
        {
            Add(nameof(TrailGroupDto.Code), "Code")
                .setMaxLength(40)
                .setUniqueOrg();

        }

        protected override async Task<bool> IsUniqueOrg(TrailGroupDto dto, string name, object value)
        {
            var entity = await _trailGroupRepo.GetByCode(dto.Code, dto.OrgNr);
            return entity == null;
        }

    }
}
