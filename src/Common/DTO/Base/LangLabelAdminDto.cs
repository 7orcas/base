
namespace Common.DTO.Base
{
    public class LangLabelAdminDto : _BaseDto
    {
        public long LangCodeId { set; get; }
        public long LangKeyId { set; get; }
        public string? Label { get; set; }
        public string? LabelEdit { get; set; }

        public bool IsDelete()
        {
            if (LabelEdit == null) return false;
            if (LabelEdit.Trim() == "") return true;
            return false;
        }

        public bool IsChanged()
        {
            if (LabelEdit == null) return false;
            if (Label == null || LabelEdit != Label) return true;
            return false;
        }

        public void Reset()
        {
            Label = null;
            LabelEdit = null;
        }
    }
}
