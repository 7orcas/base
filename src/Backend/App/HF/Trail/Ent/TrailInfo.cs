using GC = Backend.App.HF.GlobalConstantsHF;

namespace Backend.App.HF.Trail.Ent
{
    public class TrailInfo
    {
        public long Id { get; set; }
        public int Status { get; set; } = (int)GC.TrailStatus.Pending;
        public int Count { get; set; } = 0;
        
    }
}
