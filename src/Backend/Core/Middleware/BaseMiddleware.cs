namespace Backend.Core.Middleware
{
    public class BaseMiddleware
    {

        protected string GetLabel(string langKey, string nullDefault, Dictionary<string, string> labels)
        {
            if (labels.ContainsKey(langKey))
                return labels[langKey];
            return nullDefault;
        }

    }
}
