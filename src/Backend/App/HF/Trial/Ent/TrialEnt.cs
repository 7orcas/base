
/// <summary>
/// Trial Entity
/// Created: Nov 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.App.HF.Trial.Ent
{
    public class TrialEnt : BaseEntity <TrialEnt>, VersionI
    {
        public TrialEnt() { }

        public int Status { get; set; }

        public override void Decode() { }
        public override void Encode() { }
    }
}
