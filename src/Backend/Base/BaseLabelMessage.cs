using GC = Backend.GlobalConstants;
using System.Text;

/// <summary>
/// Utility class for language label messages sent from the backend and the frontend
/// Created: July 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base
{
    public class BaseLabelMessage <T> where T : BaseLabelMessage<T>
    {
        public string? initial { get; set; }
        public string labelSeparator { get; set; } = ", ";
        public StringBuilder message { get; set; } = new StringBuilder();
        public Dictionary<string, string>? labels { get; set; }
        public bool isLabelsLowerCase { get; set; } = false;

        protected T Self => (T)this;

        public BaseLabelMessage(Dictionary<string, string> labels) { 
            this.labels = labels ?? new Dictionary<string, string>(); 
        }

        public T Initialize(string initialText, string suffix)
        {
            if (labels.ContainsKey(initialText))
                initial = labels[initialText];
            else initial = initialText;
            initial += suffix;
            return Self;
        }

        public T SetLabelsLowerCase()
        {
            isLabelsLowerCase = true;
            return Self;
        }

        public T Add(string langKey)
        {
            if (message.Length > 0) message.Append(labelSeparator);
            message.Append(GetLabel(langKey));
            return Self;
        }

        public T Add(string langKey, int parameter)
        {
            if (message.Length > 0) message.Append(labelSeparator);
            var l = GetLabel(langKey);
            if (l.Contains(GC.LabelParameterPrefix))
                l = l.Replace(GC.LabelParameterPrefix, parameter.ToString());
            message.Append(l);
            return Self;
        }

        public T AddBr(string langKey)
        {
            if (message.Length > 0)
                AddWithNoSeparator("<br>");
            AddWithNoSeparator(langKey);
            return Self;
        }

        public T AddWithNoSeparator(string langKey)
        {
            message.Append(GetLabel(langKey));
            return Self;
        }

        private string GetLabel(string langKey)
        {
            var l = langKey;
            if (labels.ContainsKey(langKey))
                l = labels[langKey];
            if (isLabelsLowerCase)
                l = l.ToLower();
            return l;
        }

        public bool IsMessage()
        {
            return message.Length > 0;
        }

        public string GetMessage()
        {
            if (message.Length == 0) return "";
            if (!string.IsNullOrEmpty(initial))
            {
                return initial + message.ToString();
            }
            return message.ToString();
        }
    }
}
