namespace Backend.App.HF.TrialGroup
{
    public interface TrialGroupRepoI
    {

        Task<List<TrialGroupEnt>> GetList(SessionEnt session, TrialGroupSearch search);
        Task<TrialGroupEnt?> GetById(long trialGroupId);
        Task<TrialGroupEnt?> GetByCode(string code, int orgNr);


        Task<TrialGroupEnt> Update(SessionEnt session, TrialGroupEnt trialGroup, TrialGroupDto dto);
    }
}
