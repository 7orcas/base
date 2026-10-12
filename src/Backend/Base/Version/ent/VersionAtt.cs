namespace Backend.Base.Version.Ent
{
    [AttributeUsage (AttributeTargets.Method)]
    public class VersionAtt : Attribute
    {
        public string TableName { get; }
        public string KeyField { get; } = "Id";
        public bool IsIdKey { get; } = true;
        public bool IsLongKey { get; } = true;
        public bool IsIntegerKey { get; } = false;

        public VersionAtt(string tableName)
        {
            TableName = tableName;
        }
        public VersionAtt(string tableName, string keyField)
        {
            TableName = tableName;
            KeyField = keyField;

            if (keyField == "Nr")
            {
                IsIdKey = false;
                IsLongKey = false;
                IsIntegerKey = true;
            }
        }
    }
}
