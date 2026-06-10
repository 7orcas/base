namespace Backend.Base.Token.Ent
{
    public class TokenValues
    {
        public string IpAddress { get; set; }
        public string Username { get; set; }
        public string SessionKey { get; set; }
        public int OrgNr {  get; set; }

        public string ToLogString()
        {
            return "Username:" + Username +
                ", Org:" + OrgNr +
                ", SessionKey:" + SessionKey;
        }
    }
}
