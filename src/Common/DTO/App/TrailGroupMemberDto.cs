
namespace Common.DTO.App
{
    public class TrialGroupMemberDto : _BaseDto
    {
        public long TrialGroupId { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
