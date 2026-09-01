using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class DefinitionDto
    {
        public List<FieldDto>? Fields { get; set; }
    }

    public class FieldDto
    {
        public string Name { get; set; }
        public int? MaxLength { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public bool IsRequired { get; set; } = false;
        public bool IsRequiredNew { get; set; } = false;
    }
}
