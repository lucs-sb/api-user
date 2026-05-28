using Microsoft.EntityFrameworkCore;
using User.Infrastructure.Repositories.Configurations.Entities;

namespace User.Infrastructure.Repositories.Context;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.User> Users => Set<Domain.Entities.User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}