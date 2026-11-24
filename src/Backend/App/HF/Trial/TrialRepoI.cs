namespace Backend.App.HF.Trial
{
    public interface TrialRepoI
    {

        Task<List<TrialEnt>> GetList(SessionEnt session, TrialSearch search);
        Task<TrialEnt?> GetById(long trialId);
        Task<TrialEnt?> GetByCode(string code, int orgNr);


        Task<TrialEnt> Update(SessionEnt session, TrialEnt trial, TrialDto dto);
        Task UpdateStatus(SessionEnt session, long trialId, int status);
    }
}
