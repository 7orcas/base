using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Emit;

namespace Backend.App.HF.Trial.Ent
{
    public class TrialVal : BaseValidation <TrialDto>
    {

        private TrialRepoI _trialRepo;

        public TrialVal(SessionEnt session, 
            TrialRepoI trialRepo)
            : base(session)
        {
            _trialRepo = trialRepo;
        }

        protected override void Configure()
        {
            Add(nameof(TrialDto.Code), "Code")
                .setMaxLength(40)
                .setUniqueOrg();

            Add(nameof(TrialDto.Description), "Desc")
                .resetIsRequired()
                .setMaxLength(100);
        }

        protected override async Task<bool> IsUniqueOrg(TrialDto dto, string name, object value)
        {
            var entity = await _trialRepo.GetByCode(dto.Code, dto.OrgNr);
            return entity == null;
        }

    }
}
