using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Emit;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Org.Ent
{
    public class OrgVal : BaseValidation <OrgDto>
    {
        private OrgServiceI _orgService;

        public OrgVal(SessionEnt session,
            OrgServiceI orgService)
            : base(session) 
        {
            _orgService = orgService;
        }

        protected override void Configure()
        {
            Add(nameof(OrgDto.Nr), "OrgNr")
                .setUniqueDb();

            Add(nameof(OrgDto.Code), "Code")
                .setMaxLength(GC.LenCode)
                .setUniqueDb();

            Add(nameof(OrgDto.Description), "Description")
                .resetIsRequired()
                .setMaxLength(GC.LenDescription);

            Add(nameof(OrgDto.ApiKey), "ApiKey")
                .resetIsRequired()
                .setMaxLength(100);
        }
    }
}
