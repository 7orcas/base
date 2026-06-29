namespace Common.DTO.Base
{
    public class LoginResetActionDto
    {
        public string Token {  get; set; }
        public string Password { get; set; }
        public string LangCode { get; set; }
        public int OrgNr { get; set; }
    }
}
