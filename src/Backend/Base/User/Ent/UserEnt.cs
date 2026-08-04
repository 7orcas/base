using GC = Backend.GlobalConstants;

/// <summary>
/// User entity
/// Created: August 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.User.Ent
{

    public class UserEnt : BaseEncode
    {
        public long Id { get; set; }
        public string Username { get; set; }

        public override void Decode() { }
        public override void Encode() { }

    }


}
