namespace Backend.Base.Permission.Ent
{
    [AttributeUsage (AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class CrudAtt : Attribute
    {
        public string Action { get; }

        public CrudAtt(string crud)
        {
            Action = crud;
        }
    }
}
