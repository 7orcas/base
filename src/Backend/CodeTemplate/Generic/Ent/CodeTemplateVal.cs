using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Emit;

namespace Backend.CodeTemplate.Generic.Ent
{
    public class CodeTemplateVal : BaseValidation <CodeTemplateDto>
    {

        private CodeTemplateRepoI _codeTemplateRepo;

        public CodeTemplateVal(SessionEnt session, 
            CodeTemplateRepoI codeTemplateRepo)
            : base(session)
        {
            _codeTemplateRepo = codeTemplateRepo;
        }

        protected override void Configure()
        {
            Add(nameof(CodeTemplateDto.Code), "Code")
                .setMaxLength(40)
                .setUniqueOrg();

        }

        protected override async Task<bool> IsUniqueOrg(CodeTemplateDto dto, string name, object value)
        {
            var entity = await _codeTemplateRepo.GetByCode(dto.Code, dto.OrgNr);
            return entity == null;
        }

    }
}
