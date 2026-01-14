using ContactBookApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace ContactBookApi.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Contact> Contacts => Set<Contact>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Contact>(e =>
        {
            e.ToTable("contacts");
            e.HasKey(x => x.Id);

            e.Property(x => x.FirstName).HasMaxLength(32).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(64).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(32).IsRequired();
            e.Property(x => x.Email).HasMaxLength(254);

            e.HasIndex(x => new { x.LastName, x.FirstName });
        });
    }
}