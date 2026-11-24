using GC = Backend.App.HF.GlobalConstantsHF;

namespace Backend.App.HF.Trial.Ent
{
    public class TrialInfo
    {
        public long Id { get; set; }
        public int Status { get; set; } = (int)GC.TrialStatus.Pending;
        public int Count { get; set; } = 0;
        
    }
}
