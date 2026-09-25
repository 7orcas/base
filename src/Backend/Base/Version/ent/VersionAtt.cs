namespace Backend.Base.Version.Ent
{
    [AttributeUsage (AttributeTargets.Method)]
    public class VersionAtt : Attribute
    {
        public string TableName { get; }
        public string KeyField { get; } = "Id";

        public VersionAtt(string tableName)
        {
            TableName = tableName;
        }
        public VersionAtt(string tableName, string keyField)
        {
            TableName = tableName;
            KeyField = KeyField;
        }
    }
}
