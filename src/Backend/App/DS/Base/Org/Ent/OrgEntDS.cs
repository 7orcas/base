using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.App.DS.Base.Org.Ent
{
    public class OrgEntDS: OrgEnt
    {
        public OrgEntDS()
        {
            Encoding = new OrgEnc();
        }

        //Application URLs - Override
        [NotMapped]
        public override string AppLoginUrl { get; } = "ds-login";
        [NotMapped]
        public override string AppMfaUrl { get; } = "ds-verifyMfa";
        [NotMapped]
        public override string AppUrls { get; } = "ds-urllist";
    }
}
