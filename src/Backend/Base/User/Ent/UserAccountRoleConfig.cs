using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Base.User.Ent
{
    public class UserAccountRoleConfig : IEntityTypeConfiguration<UserAccountRoleEnt>
    {
        public void Configure(EntityTypeBuilder<UserAccountRoleEnt> entity)
        {
            entity.ToTable("useraccrole", "base");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(x => x.UserAccountId).HasColumnName("useraccid");
            entity.Property(x => x.RoleId).HasColumnName("roleid");
            entity.Property(x => x.FromDate).HasColumnName("fromdate").HasColumnType("date");
            entity.Property(x => x.ToDate).HasColumnName("todate").HasColumnType("date");
            entity.Property(x => x.IsActive).HasColumnName("isactive");
            entity.Property(x => x.Updated).HasColumnName("updated");
            entity.Property(x => x.Version).HasColumnName("version");
        }
    }
}
