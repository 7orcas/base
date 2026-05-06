using Common.DTO.Base;

namespace Common.Validator.Base
{
    public class RoleVal : _BaseVal<RoleVal, RoleDto>, ValidatorI<RoleDto>
    {
        public ValDto Validate(RoleDto dto, int orgNr, Dictionary<string, string> labels)
        {
            base.Validate(dto, orgNr, labels);

            if (val.IsError())
                val.Code = GetLabel("Role", labels) + ": " + dto.Code;

            return val;
        }

        public override string GetCodeLangKey() => "Role";

        public bool IsCodeUnique() => true;
    }
}
