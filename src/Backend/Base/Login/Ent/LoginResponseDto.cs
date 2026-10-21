namespace Backend.Base.Login.Ent
{
    public class LoginResponseDto
    {
        public string EncryptedCookie { get; set; }
        public CookieOptions CookieOptions { get; set; }
        public _ResponseDto ResponseDto { get; set; }
    }
}
