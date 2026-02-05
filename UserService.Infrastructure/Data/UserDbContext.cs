using UserService.Domain.Common;

namespace UserService.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;

/// <summary>
/// EF Core context for UserService bounded context.
/// </summary>
public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    // --- Tables ---
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);
        });
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            // --- Handle auditing ---
            if (entry.Entity is IAuditable auditable)
            {
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = now;
                    auditable.UpdatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditable.UpdatedAt = now;
                }
            }

            // --- Handle soft delete ---
            if (entry is { State: EntityState.Deleted, Entity: IDeletable deletable })
            {
                entry.State = EntityState.Modified;
                deletable.IsActive = false;

                if (entry.Entity is IAuditable auditableDeleted)
                {
                    auditableDeleted.UpdatedAt = now;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
