
/*
 * Language key (for admin)
 */
namespace Backend.Base.LabelAdmin.Ent
{
    public class LangKeyAdmin :  BaseEncode
    {
        /// <summary>
        /// The language key's id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The language key.
        /// </summary>
        public string Code { get; set; }
        public int? Variant { get; set; }

        /// <summary>
        /// The language key's description.
        /// </summary>
        public string? Description { get; set; }
                
        public DateTimeOffset Updated { get; set; }
                
        public override void Decode() { }
        public override void Encode() { }
    }
}
