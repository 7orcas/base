
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Base.Label.Ent
{
    public class LangLabel : BaseEncode
    {
        /// <summary>
        /// The label's id.
        /// </summary>
        public long Id { get; set; }

        public long LangCodeId { get; set; }
        public long LangKeyId { set; get; }



        /// <summary>
        /// Label label.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Last date/time of update.
        /// </summary>
        public DateTimeOffset Updated { get; set; }

        /// <summary>
        /// Language code of label (eg 'en').
        /// </summary>
        /*
         * Indicates this label has been loaded with this lang code
         */
        [NotMapped]
        public string? LangCode { get; set; }

        /// <summary>
        /// Language code variant number (null is the default).
        /// </summary>
        [NotMapped]
        public int? Variant { get; set; }

        /// <summary>
        /// Language key of label 
        /// </summary>
        [NotMapped]
        public string? LangKeyCode { get; set; }

        public override void Decode() { }
        public override void Encode() { }
    }
}
