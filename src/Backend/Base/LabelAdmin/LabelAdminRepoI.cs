namespace Backend.Base.LabelAdmin
{
    public interface LabelAdminRepoI
    {
        Task<List<LangCodeAdmin>> GetLangCodeList();
        Task<List<LangKeyAdmin>> GetLangKeyList();
        Task<List<LangLabelAdmin>> GetLangLabelList(string langCode);
    }
}
