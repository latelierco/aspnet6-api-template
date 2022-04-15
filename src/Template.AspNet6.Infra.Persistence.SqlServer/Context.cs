using Template.AspNet6.Domain.Entities;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Claims;
using Microsoft.EntityFrameworkCore;

namespace Template.AspNet6.Infra.Persistence.SqlServer;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Claim> Claims { get; set; } = null!;
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
                continue;

            if (entry.Entity is not ITimestampEntity entity)
                continue;

            if (entry.State == EntityState.Added)
                entity.CreatedAt = DateTime.UtcNow;

            entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
