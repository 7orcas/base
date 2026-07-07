using GC = Backend.GlobalConstants;

using Npgsql;

/// <summary>
/// Sql utilities
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Database
{
    public class SqlUtils
    {
        static public bool ValidateParameter(string parameter) => !string.IsNullOrWhiteSpace(parameter);
        
        static public long GetId(NpgsqlDataReader r) => GetId(r, "id");
        static public long GetId(NpgsqlDataReader r, string column) => (long)r[column];
        static public long? GetIdNull(NpgsqlDataReader r, string column) => r.IsDBNull(r.GetOrdinal(column)) ? null : (long)r[column];
        static public int GetOrgNr(NpgsqlDataReader r) => GetInt(r, "orgNr");


        static public int GetInt(NpgsqlDataReader r, string column) => (int)r[column];
        static public int? GetIntNull(NpgsqlDataReader r, string column) => r.IsDBNull(r.GetOrdinal(column)) ? null : (int)r[column];
        static public int GetNr(NpgsqlDataReader r) => GetInt(r, "nr");
        static public int? GetNrNull(NpgsqlDataReader r) => GetIntNull(r, "nr");


        static public string GetCode(NpgsqlDataReader r) => GetString(r, "code");
        static public string? GetDescription(NpgsqlDataReader r) => GetStringNull(r, "descr");
        static public string GetString(NpgsqlDataReader r, string column) => (string)r[column];
        static public string? GetStringNull(NpgsqlDataReader r, string column) => r.IsDBNull(r.GetOrdinal(column)) ? null : (string)r[column];

        static public string? GetEncoded(NpgsqlDataReader r) => GetStringNull(r, "encoded");

        static public Guid GetGuid(NpgsqlDataReader r, string column) => r.GetGuid(r.GetOrdinal(column));


        static public DateTime GetUpdated(NpgsqlDataReader r) => GetDateTime(r, "updated");
        static public DateTime GetDateTime(NpgsqlDataReader r, string column) => r[column] == DBNull.Value ? DateTime.MinValue : (DateTime)r[column];
        static public DateTime? GetDateTimeNull(NpgsqlDataReader r, string column) => r[column] == DBNull.Value ? null : (DateTime)r[column];


        static public bool IsActive(NpgsqlDataReader r) => GetBoolean(r, "isActive");
        static public string TestActive() => TestActive(null);
        static public string TestActive(string? table) => " " + (!string.IsNullOrEmpty(table) ? table + "." : "") + "isActive = true ";
        static public bool GetBoolean(NpgsqlDataReader r, string column) => !r.IsDBNull(r.GetOrdinal(column)) && r.GetBoolean(r.GetOrdinal(column));
        

        static public string Update(string column, int? value) => column + "=" + (value != null? value : "NULL") + ",";
        static public string Update(string column, int value) => column + "=" + value + ",";
        static public string Update(string column, string? value) => column + "=" + (value != null ? "'" + value + "'": "NULL") + ",";
        static public string Update(string column, DateTime? value) => column + "=" + (value != null ? "'" + value.Value.ToString(GC.DateTimeFormat) + "'" : "NULL") + ",";
        static public string Update(string column, bool? value) => column + "=" + (value != null ? (value.Value?"true":"false")  : "NULL") + ",";
        static public string Updatetime() => "updated='" + DateTime.Now.ToString(GC.DateTimeFormat) + "',";
        static public string NoComma(string column) => column.EndsWith(",")? column.Substring(0,column.Length-1) : column;

        static public string Insert(string value) => value == null ? "NULL" : "'" + value + "',";
        static public string Insert(int value) => value + ",";
        static public string Insert(int? value) => (value == null ? "NULL" : value) + ",";
        static public string Insert(long value) => value + ",";
        static public string Insert(bool value) => (!value? "false" : "true") + ",";
        static public string Insert(bool? value) => value == null ? "NULL" : Insert(value);

    }
}
