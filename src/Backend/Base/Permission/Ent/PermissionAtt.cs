namespace Backend.Base.Permission.Ent
{
    [AttributeUsage (AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class PermissionAtt : Attribute
    {
        public int Nr { get; }
        public string Crud { get; }

        public PermissionAtt(int nr)
        {
            Nr = nr;
        }
    }
}
