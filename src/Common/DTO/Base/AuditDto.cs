using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Base
{
    public class AuditDto : _BaseDto
    {
        public int Source { get; set; }
        public int? EntityTypeNr { get; set; }
        public string? EntityType { get; set; }
        public long? EntityId { get; set; }
        public string? UserName { get; set; }
        public string? Masquerade { get; set; }
        public string Crud { get; set; }
        public string? Details { get; set; }
    }
}
