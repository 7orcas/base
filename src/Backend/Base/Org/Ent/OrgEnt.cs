using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Organisation entity
/// Required for each customer using this application
/// Created: March 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Org.Ent
{
    public class OrgEnt : BaseEncode
    {
        public int Nr { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public DateTime Updated { get; set; }
        public bool IsActive { get; set; }
        public string? LangCode { get; set; }
        public int? LangLabelVariant { get; set; }
        
        public OrgEnc Encoding { get; set; }

        public override void Decode()
        {
            Encoding = Decode<OrgEnc>();
        }
        public override void Encode()
        {
            Encode(Encoding);
        }
    }
}
