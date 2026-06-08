namespace Backend.Base.Token.Ent
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public string? CreatedByIp { get; set; }

        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }

        public string Username { get; set; }
        public string SessionKey { get; set; }
        public int OrgNr { get; set; }

    }
}
