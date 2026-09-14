
namespace Backend.Base.Entity
{
    public interface EntityServiceI
    {
        string GetEntityTypeName(SessionEnt session, int entityId);
        List<int> GetEntityTypeNrs(SessionEnt session, string entityType, int searchType);
    }
}
