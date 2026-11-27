using Backend.App.HF.Trial.TrialGroup.Ent;

namespace Backend.App.HF.Trial.TrialGroup
{
    public interface TrialGroupRepoI
    {

        Task<List<TrialGroupEnt>> GetList(SessionEnt session, TrialGroupSearch search);
        Task<TrialGroupEnt?> GetById(long trialGroupId);
        Task<TrialGroupEnt?> GetByCode(string code, int orgNr);
        Task<List<TrialGroupMemberEnt>> GetMembers(SessionEnt session, long trialGroupId);

        Task<TrialGroupEnt> Update(SessionEnt session, TrialGroupEnt trialGroup, TrialGroupDto dto);
    }
}
