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
    public class UserAccountEnt : BaseEncode
    {
        public int OrgNr { get; set; }

        public override void Decode() { }
        public override void Encode() { }


    }
}
