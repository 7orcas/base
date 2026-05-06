using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Base
{
    public class UserRoleDto : _BaseFieldsDto<UserRoleDto>
    {
        public long RoleId { set; get; }

        //public override UserRoleDto HashMe() => HashMe(this);
        //public override bool HasChanged() => HasChanged(this);
    }
}
