using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MaharlikaGrandEstate.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Inquiry> Inquiries { get; set; }
    public DbSet<PropertyImage> PropertyImages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Property>()
        .HasOne(p => p.Buyer)
        .WithMany()
        .HasForeignKey(p => p.BuyerId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Property>()
          .Property(p => p.Price)
          .HasPrecision(18, 2);

    }
}
