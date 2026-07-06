/// <summary>
/// Utility (concrete) class for language label messages sent from the backend and the frontend
/// Created: July 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Label.Ent
{
    public class LabelMessage : BaseLabelMessage<LabelMessage>
    {
        public LabelMessage(Dictionary<string, string> labels) : base(labels)
        {
        }
    }
}
