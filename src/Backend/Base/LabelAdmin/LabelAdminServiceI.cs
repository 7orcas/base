

namespace Backend.Base.LabelAdmin
{
    public interface LabelAdminServiceI
    {
        Task<List<LangCodeAdmin>> GetLangCodeList(LabelSearch search);
        List<LangCodeAdminDto> PopulatCodes(List<LangCodeAdmin> codes);

        Task<List<LangKeyAdmin>> GetLangKeyList(LabelSearch search);
        List<LangKeyAdminDto> PopulateKeys(List<LangKeyAdmin> keys);

        Task<List<LangLabelAdmin>> GetLangLabelList(LabelSearch search);
        List<LangLabelAdminDto> PopulateLabels(List<LangLabelAdmin> labels);

        Task<LangLabel?> GetLangLabelOrCreate(LangLabelAdminDto dto);
        Task<LangLabel?> UpdateLangLabel(LangLabel langLabel, LangLabelAdminDto dto);
        LangLabelAdminDto PopulateLabel(LangLabel langLabel);

        Task<LangLabelAdminDto?> NewLangLabel(SessionEnt session, LangLabelNewDto dto);

    }
}
