namespace Common.DTO.Base
{
    public class LoginOptionDto
    {
        public string UrlSuffix { get; set; }
        public string LangCode { get; set; }
        public List<OrgDto> Orgs { get; set; }
        public List<LangCodeDto> LangCodes { get; set; }
        public int MFA { get; set; }
        public bool RememberMe { get; set; }
        public bool Forgot { get; set; }
        public bool SelfRegistration { get; set; }
        public bool Masquerade { get; set; }
    }
}
