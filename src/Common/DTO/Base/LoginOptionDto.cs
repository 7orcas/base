namespace Common.DTO.Base
{
    public class LoginOptionDto
    {
        public int LoginNr { get; set; }
        public string LangCode { get; set; }
        public List<OrgDto> Orgs { get; set; }
        public List<string> LangCodes { get; set; }
        public int MFA { get; set; }
        public bool RememberMe { get; set; }
        public bool Forgot { get; set; }
        public bool SelfRegistration { get; set; }
    }
}
