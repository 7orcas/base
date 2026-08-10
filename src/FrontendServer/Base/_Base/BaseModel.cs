
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Base controls for Dto's
/// - Hashcodes used for detecting changes
/// Created: August 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendServer.Base._Base
{
    public class BaseModel<T> where T : Common.DTO._BaseDto
    {
        public BaseModel (T _dto)
        {
            dto = _dto;
        }

        public T dto { get; private set;  }

        /// <summary>
        /// Is this Dto fully loaded?
        /// </summary>
        public bool IsLoaded { get; set; } = false;

        public string? OriginalHashCode { get; set; }


        /// <summary>
        /// Get this object's hash code (can only be run once)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T HashMe()
        {
            if (!string.IsNullOrEmpty(OriginalHashCode))
                throw new InvalidOperationException("HashCode already created");

            OriginalHashCode = GetHash(dto);
            return dto;
        }

        public T SetLoaded()
        {
            IsLoaded = true;
            return dto;
        }

        public T SetError()
        {
            IsError = true;
            return dto;
        }

        public bool HasChanged()
        {
            if (string.IsNullOrEmpty(OriginalHashCode)) return true;
            var hashCode = GetHash(dto);
            return !hashCode.Equals(OriginalHashCode);
        }

        private string GetHash(T dto)
        {
            //Remove irrelevant fields
            var o = OriginalHashCode;
            OriginalHashCode = null;
            var l = IsLoaded;
            IsLoaded = false;
            var e = IsError;
            IsError = false;

            var json = JsonSerializer.Serialize(dto);
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var hash = Convert.ToHexString(hashBytes);

            //Reset fields
            OriginalHashCode = o;
            IsLoaded = l;
            IsError = e;

            return hash;
        }

        //Convenience methods to get the underlying dto
        public long Id { get { return dto.Id; } set { dto.Id = value; } }
        public int orgNr { get { return dto.OrgNr; } set { dto.OrgNr = value; } }
        public string Code { get { return dto.Code; } set { dto.Code = value; } }
        public string? Description { get { return dto.Description; } set { dto.Description = value; } }
        public bool IsActive { get { return dto.IsActive; } set { dto.IsActive = value; } }
        public DateTimeOffset Updated { get { return dto.Updated; } set { dto.Updated = value; } }
        public int Version { get { return dto.Version; } private set { dto.Version = value; } }

        public bool IsDelete { get { return dto.IsDelete; } set { dto.IsDelete = value; } } 
        public bool IsError { get { return dto.IsError; } set { dto.IsError = value; } }
        public bool IsNew() => Id < 0;

    }
}
