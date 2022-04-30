using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Oficina300.Domain.Shops;

namespace Oficina300.Infra.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Shop> Shops { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Demand> Demands { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<Notification>();

        builder.Entity<Shop>()
            .Property(p => p.Name).IsRequired();
        builder.Entity<Shop>()
            .Property(p => p.WorkLoad).IsRequired();

        builder.Entity<Service>()
            .Property(p => p.Name).IsRequired();
        builder.Entity<Service>()
            .Property(p => p.WorkUnits).IsRequired();

        builder.Entity<Schedule>()
            .Property(c => c.Date).IsRequired();

    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
    {
        configuration.Properties<string>()
            .HaveMaxLength(100); //toda propriedade de tipo string quero q tenha no max 100 caracteres
    }
}
