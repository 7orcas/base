using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RolePermissionConfig : IEntityTypeConfiguration<RolePermissionEnt>
{
    public void Configure(EntityTypeBuilder<RolePermissionEnt> entity)
    {
        entity.ToTable("rolepermission", "base");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.RoleId).HasColumnName("roleid");
        entity.Property(e => e.PermissionNr).HasColumnName("permissionnr");
        entity.Property(e => e.Crud).HasColumnName("crud");
        entity.Property(e => e.Updated).HasColumnName("updated");
        entity.Property(e => e.IsActive).HasColumnName("isactive");

        entity.HasOne(e => e.Role)
              .WithMany(r => r.RolePermissions)
              .HasForeignKey(e => e.RoleId);
    }
}