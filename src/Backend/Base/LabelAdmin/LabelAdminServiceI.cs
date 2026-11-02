

namespace Backend.Base.LabelAdmin
{
    public interface LabelAdminServiceI
    {
        Task<List<LangCodeAdmin>> GetLangCodeList();
        List<LangCodeAdminDto> PopulatCodes(List<LangCodeAdmin> codes);
        Task<List<LangLabelAdmin>> GetLangLabelList(string langCode);
        List<LangLabelAdminDto> PopulateLabels(List<LangLabelAdmin> labels);

    }
}
