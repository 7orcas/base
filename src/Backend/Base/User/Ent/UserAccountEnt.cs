using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using GC = Backend.GlobalConstants;

/// <summary>
/// User account entity for an organisation
/// Users can have access to different organisations (each org must have a separate user account)
/// Created: August 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.User.Ent
{
    public class UserAccountEnt : BaseEncode, VersionI
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int OrgNr { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdminUser { get; set; }
        public bool IsAdminLang { get; set; }
        public int Classification { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public DateTimeOffset Updated { get; set; }
        public int Version { get; set; }


        public UserEnt User { get; set; }


        public override void Decode() { }
        public override void Encode() { }

    }
}
