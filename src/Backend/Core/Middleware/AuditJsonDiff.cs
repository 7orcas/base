using Newtonsoft.Json.Linq;
using System.Text;
using GC = Backend.GlobalConstants;

namespace Backend.Core.Middleware
{
    public static class AuditJsonDiff 
    {
        public record AuditEntry(
               string Path,
               string? Before,
               string? After);

        private static readonly HashSet<string> IgnoreFields =
                [
                    "Version",
                    "Updated",
                    "Permissions"
                ];


        public static string ToAuditEntries(JToken diff)
        {
            var auditEntries = ToAuditEntriesX(diff);
            StringBuilder sb = new StringBuilder();

            foreach (var entry in auditEntries)
            {
                var propertyName = entry.Path.Split('.').Last();
                var index = propertyName.IndexOf("[");
                if (index > 0) propertyName = propertyName.Substring(0, index - 1);

                if (IgnoreFields.Contains(propertyName))
                    continue;

                if (entry.Path.Contains(GC.Audit_Code))
                {
                    var x = entry.After;
                    index = x.IndexOf(GC.Audit_Code_prefix);
                    if (index > 0) x = x.Substring(0, index - 1);
                    sb.Append($"{entry.Path}: {x}{Environment.NewLine}");
                }
                else
                    sb.Append($"{entry.Path}: {entry.Before} -> {entry.After}{Environment.NewLine}");
            }
            return sb.ToString();
        }

        private static List<AuditEntry> ToAuditEntriesX(JToken diff)
        {
            var entries = new List<AuditEntry>();

            Process(diff, "", entries);

            return entries;
        }

        private static void Process(
            JToken token,
            string path,
            List<AuditEntry> entries)
        {
            if (token is JObject obj)
            {
                bool isArray = obj["_t"]?.Value<string>() == "a";

                foreach (var prop in obj.Properties())
                {
                    if (prop.Name == "_t")
                        continue;

                    var childPath = BuildPath(path, prop.Name, isArray);

                    Process(prop.Value, childPath, entries);
                }
            }
            else if (token is JArray arr)
            {
                // standard change
                if (arr.Count == 2)
                {
                    entries.Add(new AuditEntry(
                        path,
                        arr[0]?.ToString(),
                        arr[1]?.ToString()));
                }
                // added value
                else if (arr.Count == 1)
                {
                    entries.Add(new AuditEntry(
                        path,
                        null,
                        arr[0]?.ToString()));
                }
            }
        }

        private static string BuildPath(
            string currentPath,
            string propertyName,
            bool parentIsArray)
        {
            if (parentIsArray && int.TryParse(propertyName, out var index))
            {
                return $"{currentPath}[{index}]";
            }

            return string.IsNullOrEmpty(currentPath)
                ? propertyName
                : $"{currentPath}.{propertyName}";
        }
    }
}