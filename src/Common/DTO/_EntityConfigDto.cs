using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class _EntityConfigDto
    {
        public List<FieldConfigDto>? Fields { get; set; }
    }

    public class FieldConfigDto
    {
        public string Name { get; set; }
        public int? MaxLength { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public bool IsRequired { get; set; } = false;
        public bool IsRequiredNew { get; set; } = false;
    }
}
