using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Base
{
    public class LoginSuccessDto
    {
        //public const string TOKEN_PREFIX = "TOKEN_BLUE_";
        //public const int TOKEN_PREFIX_LENGTH = 11;

        public long Id { get; set; }
        public string TokenKey { get; set; }
        public string MainUrl { get; set; }
        public string LangCode { get; set; }
        public bool MfaRequired { get; set; }
        public bool MfaEnabled { get; set; }
    }
}
