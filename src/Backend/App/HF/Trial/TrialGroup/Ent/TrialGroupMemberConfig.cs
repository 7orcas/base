using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Base.User.Ent
{
    public class TrialGroupMemberConfig : IEntityTypeConfiguration<TrialGroupMemberEnt>
    {
        public void Configure(EntityTypeBuilder<TrialGroupMemberEnt> entity)
        {
            entity.ToTable("trialgroupuseracc", "hf");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(x => x.TrialGroupId).HasColumnName("trialgroupid");
            entity.Property(x => x.UserAccId).HasColumnName("useraccid");
            entity.Property(x => x.IsActive).HasColumnName("isactive");
            entity.Property(x => x.IsAdmin).HasColumnName("isadmin");
            entity.Property(x => x.Updated).HasColumnName("updated");
            entity.Property(x => x.Version).HasColumnName("version");
        }
    }
}
