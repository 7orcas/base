namespace Backend.Base.Audit.Ent
{
    [AttributeUsage (AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AuditIgnoreAtt : Attribute
    {
        public AuditIgnoreAtt() { }
    }
}
