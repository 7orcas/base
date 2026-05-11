namespace Backend.Data
{
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<RoleEnt> Roles => Set<RoleEnt>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            //// PostgreSQL best practice: lowercase table/column names
            //modelBuilder.Entity<RoleEnt>(entity =>
            //{
            //    entity.ToTable("role", "base");
            //    entity.Property(e => e.Id).HasColumnName("id");
            //    entity.Property(e => e.OrgNr).HasColumnName("orgnr");
            //    entity.Property(e => e.Code).HasColumnName("code");
            //    entity.Property(e => e.Description).HasColumnName("descr");
            //    entity.Property(e => e.Encoded).HasColumnName("encoded");
            //    entity.Property(e => e.Updated).HasColumnName("updated");
            //    entity.Property(e => e.IsActive).HasColumnName("isactive");
            //});

            //modelBuilder.Entity<RolePermissionEnt>(entity =>
            //{
            //    entity.ToTable("rolepermission", "base"); // ✅ match DB
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.Id).HasColumnName("id");
            //    entity.Property(e => e.RoleId).HasColumnName("roleid");
            //    entity.Property(e => e.PermissionNr).HasColumnName("permissionnr");
            //    entity.Property(e => e.Crud).HasColumnName("crud");
            //    entity.Property(e => e.Updated).HasColumnName("updated");
            //    entity.Property(e => e.IsActive).HasColumnName("isactive");


            //    entity.HasOne(e => e.Role)
            //              .WithMany(r => r.RolePermissions)
            //              .HasForeignKey(e => e.RoleId)
            //              .HasConstraintName("fk_rolepermission_role"); // optional but good


            //});

        }
    }
}
