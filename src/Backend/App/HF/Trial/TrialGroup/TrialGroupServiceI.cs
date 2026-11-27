using Backend.App.HF.Trial.TrialGroup.Ent;

namespace Backend.App.HF.Trial.TrialGroup
{
    public interface TrialGroupServiceI
    {
        Task<List<TrialGroupEnt>> GetTrialGroupList(SessionEnt session, TrialGroupSearch search);
        Task<TrialGroupEnt> GetTrialGroupById(long trialGroupId);
        Task<TrialGroupEnt?> GetTrialGroupOrCreate(SessionEnt session, TrialGroupDto dto);
        Task<TrialGroupEnt> UpdateTrialGroup(SessionEnt session, TrialGroupEnt trialGroup, TrialGroupDto dto);
        Task<TrialGroupDto?> NewTrialGroup(SessionEnt session);
        Task<List<TrialGroupMemberEnt>> GetTrialGroupMembers(SessionEnt session, long trialGroupId);

        Task<DefinitionDto> GetDefinition(SessionEnt session);
        Task<List<ValidationDto>> ValidateTrialGroups(SessionEnt session, List<TrialGroupDto> update);


        List<TrialGroupDto> Populate(SessionEnt session, List<TrialGroupEnt> entities);
        TrialGroupDto Populate(SessionEnt session, TrialGroupEnt entity);
        List<TrialGroupMemberDto> Populate(SessionEnt session, List<TrialGroupMemberEnt> entities);

    }
}
