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
        public bool IsEmailVerified { get; set; }
        public int OrgNrDefault { get; set; }
        public string LangCode { get; set; }
        public int Attempts { get; set; }
        public DateTimeOffset? AttemptsLockout { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public bool IsActive { get; set; }
        public bool IsMfaRequired { get; set; }
        public bool IsMfaEnabled { get; set; }
        public string? MfaSecret { get; set; }
        public DateTimeOffset Updated { get; set; }
        public int Version { get; set; }

        public class UserAccountDto
        {
            public int OrgNr { get; set; }
        }

    }
}
