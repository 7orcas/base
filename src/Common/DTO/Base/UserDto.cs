using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Base
{
    public class UserDto : _BaseDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string LangCode { get; set; }
        public int Attempts { get; set; }
        public DateTimeOffset? AttemptsLockout { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public bool IsMfaRequired { get; set; }
        public bool IsMfaEnabled { get; set; }
        public string? MfaSecret { get; set; }
        public List<UserAccountDto>? Accounts { get; set; }

        public class UserAccountDto : _BaseDto
        {
            public long UserId { get; set; }
            public string OrgCode { get; set; }
            public DateTimeOffset? LastLogin { get; set; }
            public int Classification { get; set; }
            public bool IsAdminUser { get; set; }
            public bool IsAdminLang { get; set; }
            public List<UserAccountRoleDto>? Roles { get; set; }
            public List<UserAccountPermissionDto>? Permissions { get; set; }
        }

        public class UserAccountRoleDto : _BaseDto
        {
            public long RoleId { get; set; }
            public string Role { get; set; }
            public bool IsRoleActive { get; set; }

        }

        public class UserAccountPermissionDto 
        {
            public int PermissionNr { get; set; }
            public string Code { get; set; }
            public string Crud { get; set; }
        }

    }
}
