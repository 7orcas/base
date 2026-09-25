
namespace Backend.Base.Entity
{

    using GC = Backend.GlobalConstants;

    public interface EntityServiceI
    {
        string GetEntityTypeName(SessionEnt session, int entityId);
        List<int> GetEntityTypeNrs(SessionEnt session, string entityType, GC.TextSearchCompare searchType);
    }
}
