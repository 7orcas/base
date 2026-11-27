

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Member entity for Trial Group
/// Created: November 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
namespace Backend.Base.User.Ent
{
    public class TrialGroupMemberEnt : VersionI
    {
        public long Id { get; set; }
        public long TrialGroupId { get; set; }
        public long UserAccId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public DateTimeOffset Updated { get; set; }
        public int Version { get; set; }

        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string Email { get; set; }

    }
}
