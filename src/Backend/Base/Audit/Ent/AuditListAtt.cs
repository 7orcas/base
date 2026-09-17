namespace Backend.Base.Audit.Ent
{
    [AttributeUsage (AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AuditListAtt : Attribute
    {
        public int EntityTypeNr { set;  get; } = -1;
        public string CrudAction { set; get; }

        public AuditListAtt() {}

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
