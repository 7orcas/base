namespace Backend.Base.Token.Ent
{
    public class TokenValues
    {
        public string Username { get; set; }
        public string SessionKey { get; set; }
        public int Org {  get; set; }

        public string ToLogString()
        {
            return "Username:" + Username +
                ", Org:" + Org +
                ", SessionKey:" + SessionKey;
        }
    }
}
