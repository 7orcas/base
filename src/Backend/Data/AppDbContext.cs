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

            // PostgreSQL best practice: lowercase table/column names
            modelBuilder.Entity<RoleEnt>(entity =>
            {
                entity.ToTable("base.role");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.OrgNr).HasColumnName("orgnr");
                entity.Property(e => e.Code).HasColumnName("code");
            });
        }
    }
}
