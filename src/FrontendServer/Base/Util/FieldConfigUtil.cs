using GC = FrontendServer.GlobalConstants;

namespace FrontendServer.Base.Util
{
    public class FieldConfigUtil
    {
        private _EntityConfigDto? EntityConfigDto;

        public FieldConfigUtil(_EntityConfigDto? EntityConfigDto) 
        {
            this.EntityConfigDto = EntityConfigDto;
        }


        public FieldConfigDto FindField (string fieldName)
        {
            if (EntityConfigDto == null) return null;
            return EntityConfigDto.Fields.FirstOrDefault(x => x.Name == fieldName);
        }

        public int MaxLength (string fieldName)
        {
            var field = FindField(fieldName);
            if (field == null || field.MaxLength == null) return GC.LenCode;
            return field.MaxLength.Value;
        }

        public bool IsRquired (string fieldName)
        {
            var field = FindField(fieldName);
            if (field == null) return false;
            return field.IsRequired;
        }

        public bool IsRquiredNew(string fieldName)
        {
            var field = FindField(fieldName);
            if (field == null) return false;
            return field.IsRequiredNew;
        }

    }
}
