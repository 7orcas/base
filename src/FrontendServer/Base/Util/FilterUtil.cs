using Newtonsoft.Json.Linq;

namespace FrontendServer.Base.Util
{
    public class FilterUtil 
    {

        public const int CompareStart = 1;
        public const int CompareContains = 2;

        private readonly Dictionary<string, Column> _columns = new();
        public event Action? Changed;


        public FilterUtil Add(string filter, string property)
        {
            Initialise(filter, property, CompareContains);
            return this;
        }

        public FilterUtil Add(string filter, string property, int compare)
        {
            Initialise(filter, property, compare);
            return this;
        }

        private void Initialise(string filter, string property, int compare)
        {
            _columns[filter] = new Column
            {
                visible = false,
                property = property,
                filter = "",
                compare = compare
            };
        }

        public bool Show(string filter)
        {
            return _columns.TryGetValue(filter, out var column) && column.visible;
        }

        public void Toggle(string filter)
        {
            if (_columns.TryGetValue(filter, out var column))
            {
                column.visible = !column.visible;
                Changed?.Invoke();
            }
        }

        public string this[string filter]
        {
            get => _columns[filter].filter;
            set
            {
                var c = _columns[filter];
                c.filter = value;
                Changed?.Invoke();
            }
        }

        public bool Filter<T>(T item)
        {
            foreach (var fieldName in _columns.Keys)
            {
                if (!Show(fieldName))
                    continue;
                
                var value = _columns[fieldName].filter;
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                var property = _columns[fieldName].property;
                var prop = typeof(T).GetProperty(property);

                if (prop == null)
                    continue;

                var dto = prop.GetValue(item)?.ToString() ?? "";

                switch (_columns[fieldName].compare)
                {
                    case CompareStart:
                        if (!dto.StartsWith(value, StringComparison.OrdinalIgnoreCase))
                            return false;
                        break;
                    case CompareContains:
                        if (!dto.Contains(value, StringComparison.OrdinalIgnoreCase))
                            return false;
                        break;
                }
            }

            return true;
        }

        private class Column
        {
            public bool visible { get; set; } = false;
            public string property { get; set; } = "";
            public string filter { get; set; } = "";
            public int compare { get; set; } = CompareContains;
        }


    }
}
