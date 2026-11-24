namespace Backend.App.HF
{
    public class GlobalConstantsHF : GlobalConstants
    {
        public const string TrialProgressHub = "trialProgressHub";
        public const string TrialProgressChanged = "trialProgressChanged";

        public enum TrialStatus
        {
            Pending = 0,
            Running = 1,
            Completed = 2,
            Failed = 3,
            Unknown = 4

        }
    }
}
