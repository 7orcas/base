using GC = Backend.GlobalConstants;

namespace Backend.Base.Entity
{
    public class EntityService : EntityServiceI
    {
        static Dictionary<int, string> EntityTypeNames; //Entity type id, Name

        public EntityService()
        {
        }


        public string GetEntityTypeName(int entityTypeId)
        {
            if (EntityTypeNames == null)
                InitialiseEntityNames();

            if (!EntityTypeNames.ContainsKey(entityTypeId))
                return "Unknown Entity Type: " + entityTypeId;

            return EntityTypeNames[entityTypeId];
        }

        private void InitialiseEntityNames()
        {
            EntityTypeNames = new Dictionary<int, string>();
            for (int i = 0; i < GC.EntityTypes.Length; i += 2)
            {
                EntityTypeNames.Add((int)GC.EntityTypes[i], (string)GC.EntityTypes[i + 1]);
            }
        }
    }
}
