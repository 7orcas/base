namespace Backend.App.HF.Trail
{
    public interface TrailRepoI
    {

        Task<List<TrailEnt>> GetList(SessionEnt session, TrailSearch search);
        Task<TrailEnt?> GetById(long trailId);
        Task<TrailEnt?> GetByCode(string code, int orgNr);


        Task<TrailEnt> Update(SessionEnt session, TrailEnt trail, TrailDto dto);
        Task UpdateStatus(SessionEnt session, long trailId, int status);
    }
}
