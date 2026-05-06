using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

/// <summary>
/// Base controls for Dto's
/// - Hashcodes used for detecting changes
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Common.DTO
{
    public abstract class _BaseDto<T> where T : _BaseDto<T>

    {
        /// <summary>
        /// Is this Dto fully loaded?
        /// </summary>
        [JsonInclude]
        public bool IsLoaded { get; set; } = false;

        [JsonInclude]
        public bool IsDelete { get; set; } = false;

        [JsonInclude]
        public string? OriginalHashCode { get; set; }

        [JsonInclude]
        public bool IsError { get; set; } = false;

        /// <summary>
        /// Get this object's hash code (can only be run once)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T HashMe()
        {
            if (!string.IsNullOrEmpty(OriginalHashCode))
                throw new InvalidOperationException("HashCode already created");
            
            OriginalHashCode = GetHash((T)(object)this);
            return (T)(object)this; 
        }
        
        public T SetLoaded()
        {
            IsLoaded = true;
            return (T)(object)this;
        }

        public T SetError()
        {
            IsError = true;
            return (T)(object)this;
        }

        public bool HasChanged()
        {
            if (string.IsNullOrEmpty(OriginalHashCode)) return true;
            var hashCode = GetHash((T)(object)this);
            return !hashCode.Equals(OriginalHashCode);
        }

        private string GetHash(T dto)
        {
            //Remove irrelevant fields
            var o = dto.OriginalHashCode;
            dto.OriginalHashCode = null;
            var l = dto.IsLoaded;
            dto.IsLoaded = false;
            var e = dto.IsError;
            dto.IsError = false;

            var json = JsonSerializer.Serialize(dto);
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var hash = Convert.ToHexString(hashBytes);
            
            //Reset fields
            dto.OriginalHashCode = o;
            dto.IsLoaded = l;
            dto.IsError = e;

            return hash;
        }

    }
}
