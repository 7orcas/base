using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Base
{
    public class VerifyEmailDto
    {
        public string Email { get; set; }
        public int OrgNr { get; set; }
        public string LangCode { get; set; }
    }
}
