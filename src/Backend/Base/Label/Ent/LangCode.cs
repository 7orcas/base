using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Language code
 */
namespace Backend.Base.Label.Ent
{
    public class LangCode :  BaseEncode
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

        public DateTime Updated { get; set; }

        /// <summary>
        /// Is language code active
        /// </summary>
        public bool IsActive { get; set; }

        public override void Decode() { }
        public override void Encode() { }
    }
}
