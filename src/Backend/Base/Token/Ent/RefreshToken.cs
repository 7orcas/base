namespace Backend.Base.Token.Ent
{
    public class RefreshToken
    {
        public long Id { get; set; }
        public Guid Token { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Expires { get; set; }
        public string? CreatedByIp { get; set; }
        public DateTimeOffset? Revoked { get; set; }
        public string? RevokedBy { get; set; }
        public string Username { get; set; }
        public string SessionKey { get; set; }
        public int OrgNr { get; set; }

        public string TokenString ()
        {
            return Token.ToString() + "-" + Id.ToString();
        }

    }
}
