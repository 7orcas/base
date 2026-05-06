namespace Backend.Base.Audit.Ent
{
    [AttributeUsage (AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AuditListAtt : Attribute
    {
        public int EntityTypeId { get; }

        public AuditListAtt(int entityTypeId)
        {
            EntityTypeId = entityTypeId;
        }
    }
}
