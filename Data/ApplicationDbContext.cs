using BilalPortfolio.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BilalPortfolio.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ContactMessage>(entity =>
        {
            entity.Property(message => message.FullName).HasMaxLength(100).IsRequired();
            entity.Property(message => message.Email).HasMaxLength(200).IsRequired();
            entity.Property(message => message.Subject).HasMaxLength(150).IsRequired();
            entity.Property(message => message.Message).HasMaxLength(4000).IsRequired();
            entity.Property(message => message.CreatedAtUtc).HasPrecision(0);
            entity.HasIndex(message => new { message.IsRead, message.CreatedAtUtc })
                .HasDatabaseName("IX_ContactMessages_IsRead_CreatedAtUtc");
        });
    }
}
