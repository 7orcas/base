namespace Backend.CodeTemplate.Generic
{
    public interface CodeTemplateRepoI
    {

        Task<List<CodeTemplateEnt>> GetList(SessionEnt session, CodeTemplateSearch search);
        Task<CodeTemplateEnt?> GetById(long codeTemplateId);
        Task<CodeTemplateEnt?> GetByCode(string code, int orgNr);


        Task<CodeTemplateEnt> Update(SessionEnt session, CodeTemplateEnt codeTemplate, CodeTemplateDto dto);
    }
}
