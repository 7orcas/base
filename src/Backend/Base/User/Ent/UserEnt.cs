using GC = Backend.GlobalConstants;

/// <summary>
/// User entity
/// Created: August 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.User.Ent
{

    public class UserEnt : BaseEncode, VersionI
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public int OrgNrDefault { get; set; }
        public string LangCode { get; set; }
        public int Attempts { get; set; }
        public DateTimeOffset? AttemptsLockout { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdminUser { get; set; }
        public bool IsMfaRequired { get; set; }
        public bool IsMfaEnabled { get; set; }
        public string? MfaSecret { get; set; }
        public DateTimeOffset Updated { get; set; }
        public int Version { get; set; }

        public ICollection<UserAccountEnt> Accounts { get; set; } = new List<UserAccountEnt>();

        public override void Decode() { }
        public override void Encode() { }

    }


}
