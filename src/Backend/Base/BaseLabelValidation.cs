/// <summary>
/// Utility class for validation language label messages sent from the backend and the frontend
/// Created: July 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base
{
    public class BaseLabelValidation : BaseLabelMessage<BaseLabelValidation>
    {

        public BaseLabelValidation(Dictionary<string, string> labels) : base (labels)
        {
        }

        public bool IsValid ()
        {
            return message.Length == 0;
        }


    }
}
