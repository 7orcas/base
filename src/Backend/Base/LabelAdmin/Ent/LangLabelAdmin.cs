
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Base.LabelAdmin.Ent
{
    public class LangLabelAdmin : BaseEncode
    {
        /// <summary>
        /// The label's id.
        /// </summary>
        public long Id { get; set; }

        public long LangCodeId { set; get; }
        public long LangKeyId { set; get; }

        /// <summary>
        /// Label key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Language code variant number (null is the default).
        /// </summary>
        public int? Variant { get; set; }

        /// <summary>
        /// Label label.
        /// </summary>
        public string Code { get; set; }
                
        /// <summary>
        /// Label tooltip.
        /// </summary>
        public string? Tooltip { get; set; }

        /// <summary>
        /// Last date/time of update.
        /// </summary>
        public DateTimeOffset Updated { get; set; }


        public override void Decode() { }
        public override void Encode() { }
    }
}
