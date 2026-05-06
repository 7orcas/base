using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Base
{
    public class LangLabelDto
    {
        public long Id { get; set; }
        public long LangKeyId { set; get; }
        public string LangKeyCode { get; set; }
        public int? Variant { get; set; }
        public string LangCode { get; set; }
        public string Label { get; set; }
        public string? Tooltip { get; set; }
        public DateTime Updated { get; set; }

        public bool IsUpdateable { get; set; } = false;
    }
}
