using System.ComponentModel.DataAnnotations;

namespace Common.Request
{
    public class SignupRequest
    {
        [Required]
        [StringLength(40)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int OrgNr { get; set; }

        [Required]
        [StringLength(4)]
        public string LangCode { get; set; }

        public bool NotRobot { get; set; }
        public string Token { get; set; }
    }

}
