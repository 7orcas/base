using Backend.Base.Label.Ent;

namespace Backend.Base.Label
{
    public interface LabelServiceI
    {
        Task<List<LangLabel>> GetLanguageLabelList(string langCode, int? variant);
        Task<Dictionary<string, LangLabel>> GetLanguageLabelDic(SessionEnt session);
        Task<Dictionary<string, LangLabel>> GetLanguageLabelDic(string langCode, int? variant);
        Task<Dictionary<string, string>> GetLangCodeDic(SessionEnt session);
        Task<Dictionary<string, string>> GetLangCodeDic(string langCode, int? variant);
        Task<List<LangLabel>> GetRelatedLabels(string langKeyCode, List<string> langCodes);
        Task<List<LangLabel>> GetAllLanguageLabels();
        Task<LangLabel> GetLanguageLabel(int id);
        Task<LangKey> GetLanguageKey(string langKeyCode);
        Task<bool> SaveLabel(LangLabel label);
    }
}
