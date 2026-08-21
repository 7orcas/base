using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using GC = Backend.GlobalConstants;

/// <summary>
/// User role for account entity for an organisation
/// Created: August 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.User.Ent
{
    public class UserAccountRoleEnt : VersionI
    {
        public long Id { get; set; }
        public long UserAccountId { get; set; }
        public long RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset Updated { get; set; }
        public int Version { get; set; }

        public UserAccountEnt UserAccount { get; set; }

    }
}
