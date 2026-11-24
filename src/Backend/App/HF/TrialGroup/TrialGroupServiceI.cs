
namespace Backend.App.HF.TrialGroup
{
    public interface TrialGroupServiceI
    {
        Task<List<TrialGroupEnt>> GetTrialGroupList(SessionEnt session, TrialGroupSearch search);
        Task<TrialGroupEnt> GetTrialGroupById(long trialGroupId);
        Task<TrialGroupEnt?> GetTrialGroupOrCreate(SessionEnt session, TrialGroupDto dto);
        Task<TrialGroupEnt> UpdateTrialGroup(SessionEnt session, TrialGroupEnt trialGroup, TrialGroupDto dto);
        Task<TrialGroupDto?> NewTrialGroup(SessionEnt session);


        Task<DefinitionDto> GetDefinition(SessionEnt session);
        Task<List<ValidationDto>> ValidateTrialGroups(SessionEnt session, List<TrialGroupDto> update);


        List<TrialGroupDto> Populate(SessionEnt session, List<TrialGroupEnt> entities);
        TrialGroupDto Populate(SessionEnt session, TrialGroupEnt entity); 

    }
}
