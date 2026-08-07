using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public abstract class _BaseVersionDto
    {
        public DateTimeOffset Updated { get; set; }
        public int Version { get; set; }
    }
}
