namespace Backend.Data
{
    using Backend.App.DS.Base.Org.Ent;
    using Backend.App.HF.Base.Org.Ent;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<OrgEnt> Orgs => Set<OrgEnt>();
        public DbSet<RoleEnt> Roles => Set<RoleEnt>();
        public DbSet<UserEnt> Users => Set<UserEnt>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //App Specific Code
            modelBuilder.Entity<OrgEnt>()
                        .HasDiscriminator(o => o.TypeOf)
                        .HasValue<OrgEnt>("OrgEnt")
                        .HasValue<OrgEntHF>("OrgEntHF")
                        .HasValue<OrgEntDS>("OrgEntDS");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
