using GC = FrontendServer.GlobalConstants;

namespace FrontendServer.Base.Util
{
    public class FieldDefinitionUtil
    {
        private DefinitionDto? definitionDto;

        public FieldDefinitionUtil(DefinitionDto? EntityConfigDto) 
        {
            this.definitionDto = EntityConfigDto;
        }


        public FieldDto FindField (string fieldName)
        {
            if (definitionDto == null) return null;
            return definitionDto.Fields.FirstOrDefault(x => x.Name == fieldName);
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
