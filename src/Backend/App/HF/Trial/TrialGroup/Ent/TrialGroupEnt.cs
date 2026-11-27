/// <summary>
/// TrialGroup Entity
/// Created: Nov 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
namespace Backend.App.HF.Trial.TrialGroup.Ent
{
    public class TrialGroupEnt : BaseEntity <TrialGroupEnt>, VersionI
    {
        public TrialGroupEnt() { }

        public ICollection<TrialGroupMemberEnt> Members { get; set; } = new List<TrialGroupMemberEnt>();

        public override void Decode() { }
        public override void Encode() { }
    }
}
