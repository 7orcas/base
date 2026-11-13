
namespace Backend.CodeTemplate.Generic
{
    public interface CodeTemplateServiceI
    {
        Task<List<CodeTemplateEnt>> GetCodeTemplateList(SessionEnt session, CodeTemplateSearch search);
        Task<CodeTemplateEnt> GetCodeTemplateById(long codeTemplateId);
        Task<CodeTemplateEnt?> GetCodeTemplateOrCreate(SessionEnt session, CodeTemplateDto dto);
        Task<CodeTemplateEnt> UpdateCodeTemplate(SessionEnt session, CodeTemplateEnt codeTemplate, CodeTemplateDto dto);
        Task<CodeTemplateDto?> NewCodeTemplate(SessionEnt session);


        Task<DefinitionDto> GetDefinition(SessionEnt session);
        Task<List<ValidationDto>> ValidateCodeTemplates(SessionEnt session, List<CodeTemplateDto> update);


        List<CodeTemplateDto> Populate(SessionEnt session, List<CodeTemplateEnt> entities);
        CodeTemplateDto Populate(SessionEnt session, CodeTemplateEnt entity); 

    }
}
