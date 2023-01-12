using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public sealed class ApplicationContext : DbContext
{
    public DbSet<ServiceObject> ServiceObjects { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        => Database.EnsureCreated();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
