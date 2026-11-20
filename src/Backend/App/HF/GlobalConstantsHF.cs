namespace Backend.App.HF
{
    public class GlobalConstantsHF : GlobalConstants
    {

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
