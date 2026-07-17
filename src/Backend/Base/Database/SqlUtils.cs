using Npgsql;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Database
{
    /// <summary>
    /// Sql utilities
    /// Created: June 2025
    /// [*Licence*]
    /// Author: John Stewart
    /// </summary>
    public class SqlUtils
    {
        /// <summary>
        /// Creates a new instance of the specified type and populates its base properties from sql.
        /// </summary>
        static public T GetBaseEntity<T>(NpgsqlDataReader r) where T : BaseEntity<T>, new()
        {
            var entity = new T();
            entity.OrgNr = GetOrgNr(r);
            entity.Id = GetId(r);
            entity.Code = GetCode(r);
            entity.Description = GetDescription(r);
            entity.Encoded = GetEncoded(r);
            //entity.Encoded = (string)r["Encoded"]; // Force Error, testing
            entity.Updated = GetUpdated(r);
            entity.Version = GetVersion(r);
            entity.IsActive = IsActive(r);
            return entity;
        }

        /// <summary>
        /// Remove trailing comma from a SQL statement if it exists
        /// </summary>
        static public string NoComma(string column) => column.EndsWith(",") ? column.Substring(0, column.Length - 1) : column;

        /// <summary>
        /// Check if a parameter is valid (not null, empty, or whitespace).
        /// </summary>
        static public bool ValidateParameter(string parameter) => !string.IsNullOrWhiteSpace(parameter);



        /// <summary>
        /// Entities id
        /// </summary>
        static public long GetId(NpgsqlDataReader r) => GetId(r, "id");

        /// <summary>
        /// Entities foreign id (not null)
        /// </summary>
        static public long GetId(NpgsqlDataReader r, string column) => (long)r[column];

        /// <summary>
        /// Entities foreign id, can be null
        /// </summary>
        static public long? GetIdNull(NpgsqlDataReader r, string column) => r.IsDBNull(r.GetOrdinal(column)) ? null : (long)r[column];

        /// <summary>
        /// Entities Org Nr (not null)
        /// </summary>
        static public int GetOrgNr(NpgsqlDataReader r) => GetInt(r, "orgNr");

        /// <summary>
        /// Get integer, not null
        /// </summary>
        static public int GetInt(NpgsqlDataReader r, string column) => (int)r[column];

        /// <summary>
        /// Get integer, can be null
        /// </summary>
        static public int? GetIntNull(NpgsqlDataReader r, string column) => r.IsDBNull(r.GetOrdinal(column)) ? null : (int)r[column];

        /// <summary>
        /// Get number field, not null
        /// </summary>
        static public int GetNr(NpgsqlDataReader r) => GetInt(r, "nr");

        /// <summary>
        /// Get number field, can be null
        /// </summary>
        static public int? GetNrNull(NpgsqlDataReader r) => GetIntNull(r, "nr");

        /// <summary>
        /// Entities version number, used for optimistic locking (not null)
        /// </summary>
        static public int GetVersion(NpgsqlDataReader r) => GetInt(r, "version");

        /// <summary>
        /// Entities unique code (not null)
        /// </summary>
        static public string GetCode(NpgsqlDataReader r) => GetString(r, "code");

        /// <summary>
        /// Entities description (can be null)
        /// </summary>
        static public string? GetDescription(NpgsqlDataReader r) => GetStringNull(r, "descr");

        /// <summary>
        /// Get string field (not null)
        /// </summary>
        static public string GetString(NpgsqlDataReader r, string column) => (string)r[column];

        /// <summary>
        /// Get string field (can be null)
        /// </summary>
        static public string? GetStringNull(NpgsqlDataReader r, string column) => r.IsDBNull(r.GetOrdinal(column)) ? null : (string)r[column];

        /// <summary>
        /// Entities encoded field, stored json key-value pairs (can be null)
        /// </summary>
        static public string? GetEncoded(NpgsqlDataReader r) => GetStringNull(r, "encoded");

        /// <summary>
        /// Get GUID field (not null)
        /// </summary>
        static public Guid GetGuid(NpgsqlDataReader r, string column) => r.GetGuid(r.GetOrdinal(column));

        /// <summary>
        /// Entities updated field, timestamp with timezone (not null)
        /// </summary>
        static public DateTimeOffset GetUpdated(NpgsqlDataReader r) => GetDateTime(r, "updated");

        /// <summary>
        /// Get date time field, timestamp with timezone (not null)
        /// </summary>
        static public DateTimeOffset GetDateTime(NpgsqlDataReader r, string column) => r[column] == DBNull.Value ? DateTime.MinValue : (DateTime)r[column];

        /// <summary>
        /// Get date time field, timestamp with timezone (can be null)
        /// </summary>
        static public DateTimeOffset? GetDateTimeNull(NpgsqlDataReader r, string column) => r[column] == DBNull.Value ? null : (DateTime)r[column];

        /// <summary>
        /// Entities active field (not null)
        /// </summary>
        static public bool IsActive(NpgsqlDataReader r) => GetBoolean(r, "isActive");

        /// <summary>
        /// Get boolean (true/false) field (false is returned if null)
        /// </summary>
        static public bool GetBoolean(NpgsqlDataReader r, string column) => !r.IsDBNull(r.GetOrdinal(column)) && r.GetBoolean(r.GetOrdinal(column));


        /// <summary>
        /// SQL Update statement for an date time column with time zones (can be null)
        /// </summary>
        static public string Set(DateTimeOffset? value) => (value != null ? "'" + value.Value.ToString(GC.DateTimeFormat) + "'" : "NULL") + ",";




        /// <summary>
        /// Update SQL statement for an entities version and timestamp
        /// </summary>
        static public string UpdateVersion<T>(T ent, string table) where T : BaseEntity<T>
        {
            return "UPDATE " + table + " " +
                   "SET " + UpdateDatetimeNow() +
                             NoComma(UpdateVersion()) + " " +
                   "WHERE id = " + ent.Id;
        }

        /// <summary>
        /// SQL Update statement for an integer column (can be null)
        /// </summary>
        static public string Update(string column, int? value) => column + "=" + (value != null? value : "NULL") + ",";

        /// <summary>
        /// SQL Update statement for an integer column (not null)
        /// </summary>
        static public string Update(string column, int value) => column + "=" + value + ",";

        /// <summary>
        /// SQL Update statement for an text column (can be null)
        /// </summary>
        static public string Update(string column, string? value) => column + "=" + (value != null ? "'" + value + "'": "NULL") + ",";
        
        /// <summary>
        /// SQL Update statement for an date time column with time zones (can be null)
        /// </summary>
        static public string Update(string column, DateTimeOffset? value) => column + "=" + (value != null ? "'" + value.Value.ToString(GC.DateTimeFormat) + "'" : "NULL") + ",";

        /// <summary>
        /// SQL Update statement for a boolean column (null values are saved as false)
        /// </summary>
        static public string Update(string column, bool? value) => column + "=" + (value != null ? (value.Value?"true":"false")  : "NULL") + ",";

        /// <summary>
        /// SQL Update statement for a date time column with time zones to NOW
        /// </summary>
        static public string UpdateDatetimeNow() => "updated='" + DateTimeOffset.Now.ToString(GC.DateTimeFormat) + "',";

        /// <summary>
        /// SQL Update statement for a date time column with time zones to NOW
        /// </summary>
        static public string UpdateVersion() => "version=version+1,";

        


        /// <summary>
        /// SQL Insert statement for an text  (can be null)
        /// </summary>
        static public string Insert(string value) => value == null ? "NULL" : "'" + value + "',";

        /// <summary>
        /// SQL Insert statement for an integer  (not null)
        /// </summary>
        static public string Insert(int value) => value + ",";

        /// <summary>
        /// SQL Insert statement for an integer  (can be null)
        /// </summary>
        static public string Insert(int? value) => (value == null ? "NULL" : value) + ",";

        /// <summary>
        /// SQL Insert statement for an long (not null)
        /// </summary>
        static public string Insert(long value) => value + ",";

        /// <summary>
        /// SQL Insert statement for an boolean (not null)
        /// </summary>
        static public string Insert(bool value) => (!value? "false" : "true") + ",";

        /// <summary>
        /// SQL Insert statement for an boolean (if null then false is used)
        /// </summary>
        static public string Insert(bool? value) => value == null ? "NULL" : Insert(value);

    }
}
