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

namespace FrontendServer.Base.Util
{
    public class ListModel<T> where T : Common.DTO._BaseDto
    {
        public ListModel (T _dto)
        {
            dto = _dto;
        }

        public T dto { get; set; }

        /// <summary>
        /// Is this Dto fully loaded?
        /// </summary>
        public bool IsLoaded { get; set; } = false;
        public bool IsSelected { get; set; } = false;

        public ListModel<T> Load(T record)
        {
            dto = record;
            IsLoaded = true;
            HashMe();
            return this;
        }


        /// <summary>
        /// Get this object's hash code (can only be run once)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ListModel<T> HashMe()
        {
            if (!string.IsNullOrEmpty(OriginalHashCode))
                throw new InvalidOperationException("HashCode already created");

            OriginalHashCode = GetHash(dto);
            return this;
        }

        public ListModel<T> ClearHash()
        {
            OriginalHashCode = null;
            return this;
        }

        public ListModel<T> SetLoaded()
        {
            IsLoaded = true;
            return this;
        }

        public ListModel<T> SetError()
        {
            IsError = true;
            return this;
        }

        public bool HasChanged()
        {
            if (string.IsNullOrEmpty(OriginalHashCode)) return true;
            var hashCode = GetHash(dto);
            return !hashCode.Equals(OriginalHashCode);
        }

        public string? OriginalHashCode { get; set; }
        private string GetHash(T dto)
        {
            var json = JsonSerializer.Serialize(dto);
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var hash = Convert.ToHexString(hashBytes);
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
