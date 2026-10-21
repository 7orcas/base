using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.App.HF.Base.Org.Ent
{
    public class OrgEntHF : OrgEnt
    {
        public OrgEntHF()
        {
            Encoding = new OrgEncHF();
        }

        public override void Decode()
        {
            Encoding = Decode<OrgEncHF>();
        }

        //Application URLs - Override
        [NotMapped]
        public override string AppLoginUrl { get; } = "hf-login";
        [NotMapped]
        public override string AppMfaUrl { get; } = "hf-verifyMfa";
        [NotMapped]
        public override string AppUrls { get; } = "hf-urllist";

    }
}
