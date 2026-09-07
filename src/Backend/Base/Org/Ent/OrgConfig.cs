using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrgConfig : IEntityTypeConfiguration<OrgEnt>
{
    public void Configure(EntityTypeBuilder<OrgEnt> entity)
    {
        entity.ToTable("org", "base");
        entity.Property(e => e.Nr).HasColumnName("nr");
        entity.Property(e => e.Code).HasColumnName("code");
        entity.Property(e => e.Description).HasColumnName("descr");
        entity.Property(e => e.Encoded).HasColumnName("encoded");
        entity.Property(e => e.Updated).HasColumnName("updated");
        entity.Property(e => e.Version).HasColumnName("version");
        entity.Property(e => e.IsActive).HasColumnName("isactive");
        entity.Property(e => e.LangCode).HasColumnName("langcode");
        entity.Property(e => e.LangLabelVariant).HasColumnName("langlabelvariant");
        entity.Property(e => e.Icon).HasColumnName("icon");
        entity.Property(e => e.ApiKey).HasColumnName("apikey");
        entity.Property(e => e.Mfa).HasColumnName("mfa");

        entity.Property(e => e.IsRememberMeEnabled).HasColumnName("isremembermeenabled");
        entity.Property(e => e.IsMasqueradeEnabled).HasColumnName("ismasqueradeenabled");
        entity.Property(e => e.IsPasswordResetEnabled).HasColumnName("ispasswordresetenabled");
        entity.Property(e => e.IsSignupEnabled).HasColumnName("issignupenabled");
        entity.Property(e => e.IsEmailRequired).HasColumnName("isemailrequired");
        entity.Property(e => e.IsEmailHtml).HasColumnName("isemailhtml");
        entity.Property(e => e.IsEmailVerified).HasColumnName("isemailverified");
       
    }
}