namespace Backend.Base.Audit.Ent
{
    [AttributeUsage (AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AuditListAtt : Attribute
    {
        public int EntityTypeNr { set;  get; } = -1;
        public string Action { set; get; }

        public AuditListAtt() {}

        public AuditListAtt(int entityTypeNr)
        {
            EntityTypeNr = entityTypeNr;
        }

        public AuditListAtt(string action)
        {
            Action = action;
        }

        public AuditListAtt(int entityTypeNr, string action)
        {
            EntityTypeNr = entityTypeNr;
            Action = action;
        }
    }
}
