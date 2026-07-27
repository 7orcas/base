using System.ComponentModel.DataAnnotations;

namespace Common.Request
{
    public class ResetRequest
    {
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(4)]
        public string LangCode { get; set; }

        public bool NotRobot { get; set; }
        public string Token { get; set; }
    }
}
