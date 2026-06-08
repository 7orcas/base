namespace Common.DTO.Base
{
    public class LoginTokenDto
    {

        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
        public string RefreshToken { get; set; }

    }
}
