using Npgsql;
using GC = Backend.GlobalConstants;

namespace Backend.Base;

public abstract class BaseService : SqlUtils
{
    protected readonly Serilog.ILogger _log;
    public AuditServiceI _auditService;

    public BaseService(IServiceProvider serviceProvider) 
    { 
        _log = Log.Logger;

        //for testing
        //if (serviceProvider == null) 
        //    return;

        // Create a scoped service provider
        using var scope = serviceProvider.CreateScope();
        _auditService = scope.ServiceProvider.GetRequiredService<AuditServiceI>();
    }

    public T ReadBaseEntity<T>(NpgsqlDataReader r) where T : BaseEntity<T>, new()
    {
        var entity = new T();
        entity.OrgNr = GetOrgNr(r);
        entity.Id = GetId(r);
        entity.Code = GetCode(r);
        entity.Description = GetDescription(r);
        entity.Encoded = GetEncoded(r);
        //entity.Encoded = (string)r["Encoded"]; // Force Error, testing
        entity.Updated = GetUpdated(r);
        entity.IsActive = IsActive(r);

        return entity;
    }

    public void TestDelaySeconds (int seconds)
    {
        System.Threading.Thread.Sleep(seconds * 1000); 
    }

    protected string Update<E> (BaseEntity<E> ent)
    {
        return Update("orgNr", ent.OrgNr) + 
            Update("code", ent.Code) + 
            Update("descr", ent.Description) + 
            Update("encoded", ent.Encoded) + 
            Update("updated", DateTime.Now) +
            NoComma(Update("isActive", ent.IsActive));
    }

    protected string Insert<E>(BaseEntity<E> ent)
    {
        string? e = null;
        if (ent is BaseEncode be)
            e = be.Encoded;
        
        return "(orgNr," +
            "code," +
            (ent.Description != null? "descr," : "") +
            (e != null ? "encoded," : "") +
            "updated," +
            "isActive) " +

            "VALUES (" +
            ent.OrgNr + "," +
            "'" + ent.Code + "'," +
            (ent.Description != null ? "'" + ent.Description + "'," : "") +
            (e != null ? "'" + e + "'," : "") +
            "'" + DateTime.Now.ToString(GC.DateTimeFormat) + "'," +
            (ent.IsActive?"true":"false") +
            ")";
    }

    protected string GetLabel(string langKey, Dictionary<string, string> labels) => GetLabel(langKey, langKey, labels);
    
    protected string GetLabel(string langKey, string nullDefault, Dictionary<string, string> labels)
    {
        if (labels.ContainsKey(langKey))
            return labels[langKey];
        return nullDefault;
    }

}
