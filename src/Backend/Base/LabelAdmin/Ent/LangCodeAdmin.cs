
/*
 * Language code (for admin)
 */
namespace Backend.Base.LabelAdmin.Ent
{
    public class LangCodeAdmin :  BaseEncode
    {
        /// <summary>
        /// The language code's id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The language code's id.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The language code's description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The language code's flag.
        /// </summary>
        public string? Flag { get; set; }

        public DateTimeOffset Updated { get; set; }

        /// <summary>
        /// Is language code active
        /// </summary>
        public bool IsActive { get; set; }

        public override void Decode() { }
        public override void Encode() { }
    }
}
