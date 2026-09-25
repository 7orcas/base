using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using GC = Backend.GlobalConstants;

namespace Backend.Core.Middleware
{
    public static class AuditCodeCleaner
    {
        public static void NullAuditCode(object? obj)
        {
            if (obj == null)
                return;

            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
            NullAuditCodeRecursive(obj, visited);
        }

        private static void NullAuditCodeRecursive(
            object obj,
            HashSet<object> visited)
        {
            if (obj == null || !visited.Add(obj))
                return;

            // If the root object is a collection, iterate it
            if (obj is IEnumerable list && obj is not string)
            {
                foreach (var item in list)
                {
                    if (item != null)
                        NullAuditCodeRecursive(item, visited);
                }

                return;
            }

            var type = obj.GetType();

            // Skip primitive types
            if (type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(string) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(decimal) ||
                type == typeof(Guid))
            {
                return;
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanRead)
                    continue;

                // Skip indexers
                if (property.GetIndexParameters().Length > 0)
                    continue;

                // Null Audit_Code
                if (property.Name == GC.Audit_Code 
                    && property.CanWrite
                    && property.PropertyType == typeof(string))
                {
                    property.SetValue(obj, null);
                    continue;
                }

                var value = property.GetValue(obj);

                if (value == null || value is string)
                    continue;

                // Recurse collections
                if (value is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item != null)
                            NullAuditCodeRecursive(item, visited);
                    }
                }
                else if(!property.PropertyType.IsValueType)
                {
                    NullAuditCodeRecursive(value, visited);
                }
            }
        }
    }

    /// <summary>
    /// Used to prevent infinite loops from circular references.
    /// </summary>
    public sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance = new();

        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
