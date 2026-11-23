
namespace Backend.App.HF.Trail
{
    public interface TrailServiceI
    {
        Task<List<TrailEnt>> GetTrailList(SessionEnt session, TrailSearch search);
        Task<TrailEnt> GetTrailById(long trailId);
        Task<TrailEnt?> GetTrailOrCreate(SessionEnt session, TrailDto dto);
        Task<TrailEnt> UpdateTrail(SessionEnt session, TrailEnt trail, TrailDto dto);
        Task<TrailDto?> NewTrail(SessionEnt session);


        Task<TrailConfigDto> GetConfiguration(SessionEnt session);
        Task<DefinitionDto> GetDefinition(SessionEnt session);
        Task<List<ValidationDto>> ValidateTrails(SessionEnt session, List<TrailDto> update);


        List<TrailDto> Populate(SessionEnt session, List<TrailEnt> entities);
        TrailDto Populate(SessionEnt session, TrailEnt entity);
        void Populate(SessionEnt session, TrailInfo info, TrailDto dto);


        Task<TrailInfo> StartTrail(SessionEnt session, TrailDto dto);
        Task<TrailInfo?> GetTrail(long id);

    }
}
