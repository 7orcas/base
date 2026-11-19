
namespace Backend.App.HF.TrailGroup
{
    public interface TrailGroupServiceI
    {
        Task<List<TrailGroupEnt>> GetTrailGroupList(SessionEnt session, TrailGroupSearch search);
        Task<TrailGroupEnt> GetTrailGroupById(long trailGroupId);
        Task<TrailGroupEnt?> GetTrailGroupOrCreate(SessionEnt session, TrailGroupDto dto);
        Task<TrailGroupEnt> UpdateTrailGroup(SessionEnt session, TrailGroupEnt trailGroup, TrailGroupDto dto);
        Task<TrailGroupDto?> NewTrailGroup(SessionEnt session);


        Task<DefinitionDto> GetDefinition(SessionEnt session);
        Task<List<ValidationDto>> ValidateTrailGroups(SessionEnt session, List<TrailGroupDto> update);


        List<TrailGroupDto> Populate(SessionEnt session, List<TrailGroupEnt> entities);
        TrailGroupDto Populate(SessionEnt session, TrailGroupEnt entity); 

    }
}
