using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.App.DS.Base.Org.Ent
{
    public class OrgEntDS: OrgEnt
    {
        public OrgEntDS()
        {
            Encoding = new OrgEnc();
        }
    }
}
