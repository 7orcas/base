using Common.DTO;
using GC = Common.GlobalConstants;

/// <summary>
/// Base validation utilities
/// Created: July 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Common.Validator
{
    public abstract class _BaseVal<T, D> where T : _BaseVal<T,D> where D : _BaseFieldsDto<D>
    {
        public ValDto val {  get; } = new ValDto();

        public ValDto Validate(D dto, int? orgNr, Dictionary<string, string> labels) 
        {
            val.Id = dto.Id;

            if (orgNr != null && 
                (dto.orgNr == null || dto.orgNr != orgNr))
                val.AddError(GetLabel("OrgNr", labels), GetLabel("InvO", labels));

            if (dto.Code == null || 
                string.IsNullOrWhiteSpace(dto.Code) ||
                dto.Code.EndsWith(" "))
                val.AddError(GetLabel(GetCodeLangKey(), labels), GetLabel("InvF", labels));

            if (dto.Code != null && dto.Code.Length > GC.LenCode)
                val.AddError(GetLabel(GetCodeLangKey(), labels), GetLabel("InvL", labels, GC.LenCode));



            return val;
        }

        public virtual string GetCodeLangKey()
        {
            return "Code";
        }


        protected string GetLabel (string key, Dictionary<string, string> labels)
        {
            if (labels == null) return key;
            if (labels.TryGetValue(key, out var v))
                return v;
            return key;
        }

        protected string GetLabel(string key, Dictionary<string, string> labels, int parameter)
        {
            if (labels == null) return key;
            if (labels.TryGetValue(key, out var v))
                return v.Replace(GC.LabelParameterPrefix, "" + parameter);
            return key;
        }

    }
}
