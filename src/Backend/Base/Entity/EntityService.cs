using static Common.GlobalConstants;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Entity
{
    public class EntityService : EntityServiceI
    {
        static Dictionary<int, string> EntityTypeNames; //Entity type id, Name

        public EntityService()
        {
        }


        public string GetEntityTypeName(SessionEnt session, int entityTypeNr)
        {
            //To Do : need to factor in the application type (eg FyH, FMM)

            if (EntityTypeNames == null)
                InitialiseEntityNames(session);

            if (!EntityTypeNames.ContainsKey(entityTypeNr))
                return "Unknown Entity Type: " + entityTypeNr;

            return EntityTypeNames[entityTypeNr];
        }

        public List<int> GetEntityTypeNrs(SessionEnt session, string entityType, TextSearchCompare searchType)
        {
            //To Do : need to factor in the application type (eg FyH, FMM)

            if (EntityTypeNames == null)
                InitialiseEntityNames(session);

            return EntityTypeNames
                .Where(x => IsMatch(x.Value, entityType, searchType))
                .Select(x => x.Key)
                .ToList();
        }

        private static bool IsMatch(string source, string value, GC.TextSearchCompare searchType)
        {
            if (string.IsNullOrWhiteSpace(source))
                return false;

            switch (searchType)
            {
                case GC.TextSearchCompare.Start:
                    return source.StartsWith(value, StringComparison.OrdinalIgnoreCase);

                case GC.TextSearchCompare.Contains:
                    return source.Contains(value, StringComparison.OrdinalIgnoreCase);

                default:
                    return source.Equals(value, StringComparison.OrdinalIgnoreCase);
            }
        }


        private void InitialiseEntityNames(SessionEnt session)
        {
            EntityTypeNames = new Dictionary<int, string>();
            for (int i = 0; i < GC.EntityTypes.Length; i += 2)
            {
                EntityTypeNames.Add((int)GC.EntityTypes[i], (string)GC.EntityTypes[i + 1]);
            }
        }
    }
}
