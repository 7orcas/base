using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Base
{
    public class UserDto : _BaseDto<UserDto>
    {
        public long UserId { get; set; }
        public string Username { get; set; }


        public class UserAccountDto
        {
            public int OrgNr { get; set; }
        }

    }
}
