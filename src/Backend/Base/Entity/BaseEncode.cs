using Newtonsoft.Json;

/// <summary>
/// Base entity using encodings
/// Contains utility functions
/// Created: March 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Entity
{
    public abstract class BaseEncode
    {
        public string Encoded { get; set; } = string.Empty;
        public abstract void Decode();
        public abstract void Encode();

        protected T Decode<T>() where T : new()
        {
            if (Encoded == null) return new T();
            return JsonConvert.DeserializeObject<T>(Encoded) ?? new T();
        }

        public void Encode<T>(T enc)
        {
            Encoded = JsonConvert.SerializeObject(enc);
        }
    }
}
