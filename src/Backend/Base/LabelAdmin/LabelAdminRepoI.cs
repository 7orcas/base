namespace Backend.Base.LabelAdmin
{
    public interface LabelAdminRepoI
    {
        Task<List<LangCodeAdmin>> GetLangCodeList(LabelSearch search);
        Task<List<LangKeyAdmin>> GetLangKeyList(LabelSearch search);
        Task<List<LangLabelAdmin>> GetLangLabelList(LabelSearch search);
        Task<long> IsExist(LangLabelNewDto dto);
        Task<LangLabelAdminDto> NewLangLabel(LangLabelNewDto dto);
    }
}
