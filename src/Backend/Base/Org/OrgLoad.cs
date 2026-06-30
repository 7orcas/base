using GC = Backend.GlobalConstants;
using Npgsql;

/// <summary>
/// Utility class to load org entities.
/// Used by singleton service and other services (keeps the DRY princple)
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Org
{
    public class OrgLoad : SqlUtils
    {
        static public OrgEnt Load(NpgsqlDataReader r)
        {
            var org = new OrgEnt();
            org.Nr = GetInt(r, "nr");
            org.Code = GetCode(r);
            org.Description = GetDescription(r);
            org.Icon = GetStringNull(r, "icon");
            org.Updated = GetUpdated(r);
            org.IsActive = IsActive(r);
            org.LangLabelVariant = GetIntNull(r, "langLabelVariant");
            org.Encoded = GetEncoded(r);
            org.MfaRequired = GetBoolean(r, "mfaRequired");
            org.Forgotenabled = GetBoolean(r, "forgotEnabled");
            org.EmailRequired = GetBoolean(r, "emailRequired");
            org.Decode();

            if (org.LangCode == null) org.LangCode = GC.LangCodeDefault;

            return org;
        }
    }
}
