using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfig : IEntityTypeConfiguration<UserEnt>
{
    public void Configure(EntityTypeBuilder<UserEnt> entity)
    {
        entity.ToTable("zzz", "base");
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.Username).HasColumnName("xxx");
        entity.Property(e => e.Email).HasColumnName("email");
        entity.Property(e => e.Encoded).HasColumnName("encoded");
        entity.Property(e => e.Updated).HasColumnName("updated");
        entity.Property(e => e.IsActive).HasColumnName("isactive");

        entity.Property(e => e.IsEmailVerified).HasColumnName("isemailverified");
        entity.Property(e => e.OrgNrDefault).HasColumnName("orgnrdefault");
        entity.Property(e => e.LangCode).HasColumnName("langcode");
        entity.Property(e => e.Attempts).HasColumnName("attempts");
        entity.Property(e => e.AttemptsLockout).HasColumnName("attemptslockout");
        entity.Property(e => e.LastLogin).HasColumnName("lastlogin");
        entity.Property(e => e.IsMfaRequired).HasColumnName("ismfarequired");
        entity.Property(e => e.IsMfaEnabled).HasColumnName("ismfaenabled");
        entity.Property(e => e.MfaSecret).HasColumnName("mfasecret");
        entity.Property(e => e.Version).HasColumnName("version");


        entity.HasMany(u => u.Accounts)
              .WithOne(a => a.User)
              .HasForeignKey(a => a.UserId);

    }
}