namespace Backend.App.HF.TrailGroup
{
    public interface TrailGroupRepoI
    {

        Task<List<TrailGroupEnt>> GetList(SessionEnt session, TrailGroupSearch search);
        Task<TrailGroupEnt?> GetById(long trailGroupId);
        Task<TrailGroupEnt?> GetByCode(string code, int orgNr);


        Task<TrailGroupEnt> Update(SessionEnt session, TrailGroupEnt trailGroup, TrailGroupDto dto);
    }
}
