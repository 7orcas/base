using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Base.User.Ent
{
    public class UserAccountConfig : IEntityTypeConfiguration<UserAccountEnt>
    {
        public void Configure(EntityTypeBuilder<UserAccountEnt> entity)
        {
            entity.ToTable("useracc", "base");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(x => x.UserId).HasColumnName("zzzid");
            entity.Property(x => x.OrgNr).HasColumnName("orgnr");
            entity.Property(e => e.Encoded).HasColumnName("encoded");
            entity.Property(x => x.IsActive).HasColumnName("isactive");
            entity.Property(x => x.IsAdminUser).HasColumnName("isadminuser");
            entity.Property(x => x.IsAdminLang).HasColumnName("isadminlang");
            entity.Property(x => x.Classification).HasColumnName("classification");
            entity.Property(x => x.LastLogin).HasColumnName("lastlogin");
            entity.Property(x => x.Updated).HasColumnName("updated");
            entity.Property(x => x.Version).HasColumnName("version");
        }
    }
}
