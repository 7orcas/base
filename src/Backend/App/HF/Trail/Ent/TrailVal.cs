using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Emit;

namespace Backend.App.HF.Trail.Ent
{
    public class TrailVal : BaseValidation <TrailDto>
    {

        private TrailRepoI _trailRepo;

        public TrailVal(SessionEnt session, 
            TrailRepoI trailRepo)
            : base(session)
        {
            _trailRepo = trailRepo;
        }

        protected override void Configure()
        {
            Add(nameof(TrailDto.Code), "Code")
                .setMaxLength(40)
                .setUniqueOrg();

            Add(nameof(TrailDto.Description), "Desc")
                .resetIsRequired()
                .setMaxLength(100);
        }

        protected override async Task<bool> IsUniqueOrg(TrailDto dto, string name, object value)
        {
            var entity = await _trailRepo.GetByCode(dto.Code, dto.OrgNr);
            return entity == null;
        }

    }
}
