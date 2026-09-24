using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace Backend.Core.Middleware
{
    public static class AuditCodeDELETE_ME
    {
        public static void ProcessChangedObjects(
            object? before,
            object? after,
            Action<object> onChanged)
        {
            if (before == null || after == null)
                return;

            ProcessChangedObjectsInternal(
                before,
                after,
                onChanged,
                new HashSet<object>(ReferenceEqualityComparer.Instance));
        }

        private static void ProcessChangedObjectsInternal(
            object before,
            object after,
            Action<object> onChanged,
            HashSet<object> visited)
        {
            if (!visited.Add(after))
                return;

            bool objectChanged = false;

            var type = after.GetType();

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead)
                    continue;

                var beforeValue = prop.GetValue(before);
                var afterValue = prop.GetValue(after);

                if (IsSimpleType(prop.PropertyType))
                {
                    if (!Equals(beforeValue, afterValue))
                    {
                        objectChanged = true;
                    }

                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType)
                    && prop.PropertyType != typeof(string))
                {
                    ProcessCollection(
                        beforeValue as IEnumerable,
                        afterValue as IEnumerable,
                        onChanged,
                        visited);

                    continue;
                }

                if (beforeValue == null && afterValue == null)
                    continue;

                if (beforeValue == null || afterValue == null)
                {
                    objectChanged = true;
                    continue;
                }

                if (HasDirectPropertyChanges(beforeValue, afterValue))
                {
                    objectChanged = true;
                }

                ProcessChangedObjectsInternal(
                    beforeValue,
                    afterValue,
                    onChanged,
                    visited);
            }

            if (objectChanged)
            {
                onChanged(after);
            }
        }

        private static void ProcessCollection(
            IEnumerable? beforeItems,
            IEnumerable? afterItems,
            Action<object> onChanged,
            HashSet<object> visited)
        {
            var beforeById = (beforeItems ?? Enumerable.Empty<object>())
                .Cast<object>()
                .ToDictionary(GetId);

            var afterById = (afterItems ?? Enumerable.Empty<object>())
                .Cast<object>()
                .ToDictionary(GetId);

            // Updated items
            foreach (var (id, afterItem) in afterById)
            {
                if (beforeById.TryGetValue(id, out var beforeItem))
                {
                    ProcessChangedObjectsInternal(
                        beforeItem,
                        afterItem,
                        onChanged,
                        visited);
                }
                else
                {
                    // New item
                    onChanged(afterItem);
                }
            }

            // Deleted items
            foreach (var (id, beforeItem) in beforeById)
            {
                if (!afterById.ContainsKey(id))
                {
                    onChanged(beforeItem);
                }
            }
        }

        private static bool HasDirectPropertyChanges(
            object before,
            object after)
        {
            foreach (var prop in after.GetType().GetProperties())
            {
                if (!prop.CanRead)
                    continue;

                if (!IsSimpleType(prop.PropertyType))
                    continue;

                var beforeValue = prop.GetValue(before);
                var afterValue = prop.GetValue(after);

                if (!Equals(beforeValue, afterValue))
                {
                    return true;
                }
            }

            return false;
        }

        private static long GetId(object obj)
        {
            return ((_BaseDto)obj).Id;
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(Guid);
        }
    }
}