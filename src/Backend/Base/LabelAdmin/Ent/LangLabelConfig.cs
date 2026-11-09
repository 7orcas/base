using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LangLabelConfig : IEntityTypeConfiguration<LangLabel>
{
    public void Configure(EntityTypeBuilder<LangLabel> entity)
    {
        entity.ToTable("langlabel", "base");
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.LangCodeId).HasColumnName("langcodeid");
        entity.Property(e => e.LangKeyId).HasColumnName("langkeyid");
        entity.Property(e => e.Code).HasColumnName("code");
        entity.Property(e => e.Encoded).HasColumnName("encoded");
        entity.Property(e => e.Updated).HasColumnName("updated");
    }
}