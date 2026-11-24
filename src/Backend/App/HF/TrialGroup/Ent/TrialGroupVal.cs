using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Emit;

namespace Backend.App.HF.TrialGroup.Ent
{
    public class TrialGroupVal : BaseValidation <TrialGroupDto>
    {

        private TrialGroupRepoI _trialGroupRepo;

        public TrialGroupVal(SessionEnt session, 
            TrialGroupRepoI trialGroupRepo)
            : base(session)
        {
            _trialGroupRepo = trialGroupRepo;
        }

        protected override void Configure()
        {
            Add(nameof(TrialGroupDto.Code), "Code")
                .setMaxLength(40)
                .setUniqueOrg();

        }

        protected override async Task<bool> IsUniqueOrg(TrialGroupDto dto, string name, object value)
        {
            var entity = await _trialGroupRepo.GetByCode(dto.Code, dto.OrgNr);
            return entity == null;
        }

    }
}
