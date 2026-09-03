namespace Backend.Base.Audit.Ent
{
    [AttributeUsage (AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AuditListAtt : Attribute
    {
        public int EntityTypeId { get; }
        public string CrudAction { get; }

        public AuditListAtt(int entityTypeId)
        {
            EntityTypeId = entityTypeId;
        }

        public AuditListAtt(string crudAction)
        {
            CrudAction = crudAction;
        }

        public AuditListAtt(int entityTypeId, string crudAction)
        {
            EntityTypeId = entityTypeId;
            CrudAction = crudAction;
        }
    }
}
