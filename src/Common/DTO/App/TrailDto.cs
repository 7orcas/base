
namespace Common.DTO.App
{
    public class TrialDto : _BaseDto
    {
        public long TrialGroupId { get; set; }
        public int Status { get; set; }
        public string StatusDescription { get; set; }
        public int Count { get; set; }
    }
}
