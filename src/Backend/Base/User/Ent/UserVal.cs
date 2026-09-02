using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Emit;

namespace Backend.Base.User.Ent
{
    public class UserVal : BaseValidation <UserDto>
    {
        private OrgServiceI _orgService;

        public UserVal(SessionEnt session,
            OrgServiceI orgService)
            : base(session) 
        {
            _orgService = orgService;
        }

        protected override void Configure()
        {
            Add(nameof(UserDto.Username), "UserName")
                .setMaxLength(40)
                .setUniqueDb();

            Add(nameof(UserDto.PasswordNew), "PW")
                .setMaxLength(100)
                .resetIsRequired()
                .setIsRequiredNew()
                .setCallBack(ValidatePW);

            Add(nameof(UserDto.Email), "Email")
                .setMaxLength(100)
                .setCallBack(ValidateEmail)
                .IsRequired = org.IsEmailRequired;

            Add(nameof(UserDto.LangCode), "LangCode")
                .setMaxLength(4);

        }

        public void ValidateEmail (UserDto dto)
        {
            if (!org.IsEmailRequired) return;

            if (!string.IsNullOrEmpty(dto.Email) && !IsValidEmail(dto.Email))
                AddMessage(GetLabel("Email", "Email"), GetLabel("Invalid", "Invalid"));
        }

        public void ValidatePW(UserDto dto)
        {
            if (string.IsNullOrEmpty(dto.PasswordNew) || _orgService == null)
                return;

            var r = _orgService.ValidatePassword(dto.PasswordNew, org, labels);
            if (!r.valid)
                AddMessage(GetLabel("PWn", "New Password"), r.message);
        }
    }
}
