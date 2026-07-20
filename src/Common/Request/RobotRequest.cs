using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Request
{
    public class RobotRequest
    {
        public string LangCode { get; set; }
        public int AppClient { get; set; }
        public string CaptchaToken { get; set; }
    }
}
