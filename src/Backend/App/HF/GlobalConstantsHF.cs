namespace Backend.App.HF
{
    public class GlobalConstantsHF : GlobalConstants
    {
        public const string TrailProgressHub = "trailProgressHub";
        public const string TrailProgressChanged = "trailProgressChanged";

        public enum TrailStatus
        {
            Pending = 0,
            Running = 1,
            Completed = 2,
            Failed = 3,
            Unknown = 4

        }
    }
}
