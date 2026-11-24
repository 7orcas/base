
namespace Backend.App.HF.Trial
{
    public interface TrialServiceI
    {
        Task<List<TrialEnt>> GetTrialList(SessionEnt session, TrialSearch search);
        Task<TrialEnt> GetTrialById(long trialId);
        Task<TrialEnt?> GetTrialOrCreate(SessionEnt session, TrialDto dto);
        Task<TrialEnt> UpdateTrial(SessionEnt session, TrialEnt trial, TrialDto dto);
        Task<TrialDto?> NewTrial(SessionEnt session);


        Task<TrialConfigDto> GetConfiguration(SessionEnt session);
        Task<DefinitionDto> GetDefinition(SessionEnt session);
        Task<List<ValidationDto>> ValidateTrials(SessionEnt session, List<TrialDto> update);


        List<TrialDto> Populate(SessionEnt session, List<TrialEnt> entities);
        TrialDto Populate(SessionEnt session, TrialEnt entity);
        void Populate(SessionEnt session, TrialInfo info, TrialDto dto);


        Task<TrialInfo> StartTrial(SessionEnt session, TrialDto dto);
        Task<TrialInfo?> GetTrial(long id);

    }
}
