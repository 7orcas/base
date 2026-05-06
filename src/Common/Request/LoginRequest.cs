using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Request
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string LangCode { get; set; }
        public int Org { get; set; }
        public int SourceApplication { get; set; }

        public bool IsPopulated()
        {
            return !string.IsNullOrEmpty(Username)
                && !string.IsNullOrEmpty(Password)
                && !string.IsNullOrEmpty(LangCode);
        }
    }
}
