using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Base
{
    public class UserDto : _BaseDto<UserDto>
    {
        public long Id { get; set; }
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
        public List<UserAccountDto>? Accounts { get; set; }

        public bool IsNew() => Id < 0;

        public class UserAccountDto : _BaseVersionDto
        {
            public long Id { get; set; }
            public long UserId { get; set; }
            public int OrgNr { get; set; }
            public string OrgCode { get; set; }
            public DateTimeOffset? LastLogin { get; set; }
            public int Classification { get; set; }
            public bool IsActive { get; set; }
            public bool IsAdminUser { get; set; }
            public bool IsAdminLang { get; set; }
            public List<UserAccountRoleDto>? Roles { get; set; }
        }

        public class UserAccountRoleDto : _BaseVersionDto
        {
            public long Id { get; set; }
            public long RoleId { get; set; }
            public string Role { get; set; }
            public bool IsActive { get; set; }

        }



    }
}
