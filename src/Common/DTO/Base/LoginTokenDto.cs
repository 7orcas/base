using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Base
{
    public class LoginTokenDto
    {
        public const string TOKEN_PREFIX = "TOKEN_BLUE_";
        public const int TOKEN_PREFIX_LENGTH = 11;

        public string TokenKey { get; set; }
        public string Token { get; set; }
        public string MainUrl { get; set; }
        public string LangCode { get; set; }
    }
}
