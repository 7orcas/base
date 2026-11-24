using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TrialConfig : IEntityTypeConfiguration<TrialEnt>
{
    public void Configure(EntityTypeBuilder<TrialEnt> entity)
    {
        entity.ToTable("trial", "hf");
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.Status).HasColumnName("status");
        entity.Property(e => e.OrgNr).HasColumnName("orgnr");
        entity.Property(e => e.Code).HasColumnName("code");
        entity.Property(e => e.Description).HasColumnName("descr");
        entity.Property(e => e.Encoded).HasColumnName("encoded");
        entity.Property(e => e.Updated).HasColumnName("updated");
        entity.Property(e => e.Version).HasColumnName("version");
        entity.Property(e => e.IsActive).HasColumnName("isactive");
    }
}