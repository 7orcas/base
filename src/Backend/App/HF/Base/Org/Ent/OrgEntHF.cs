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
       
    }
}
