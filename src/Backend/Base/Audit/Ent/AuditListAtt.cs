namespace Backend.Base.Audit.Ent
{
    [AttributeUsage (AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AuditListAtt : Attribute
    {
        public int EntityTypeNr { get; } = -1;
        public string CrudAction { get; }

        public AuditListAtt(int entityTypeNr)
        {
            EntityTypeNr = entityTypeNr;
        }

        public AuditListAtt(string crudAction)
        {
            CrudAction = crudAction;
        }

        public AuditListAtt(int entityTypeNr, string crudAction)
        {
            EntityTypeNr = entityTypeNr;
            CrudAction = crudAction;
        }
    }
}
