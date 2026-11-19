using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TrailGroupConfig : IEntityTypeConfiguration<TrailGroupEnt>
{
    public void Configure(EntityTypeBuilder<TrailGroupEnt> entity)
    {
        entity.ToTable("trailgroup", "hf");
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.OrgNr).HasColumnName("orgnr");
        entity.Property(e => e.Code).HasColumnName("code");
        entity.Property(e => e.Description).HasColumnName("descr");
        entity.Property(e => e.Encoded).HasColumnName("encoded");
        entity.Property(e => e.Updated).HasColumnName("updated");
        entity.Property(e => e.Version).HasColumnName("version");
        entity.Property(e => e.IsActive).HasColumnName("isactive");
    }
}