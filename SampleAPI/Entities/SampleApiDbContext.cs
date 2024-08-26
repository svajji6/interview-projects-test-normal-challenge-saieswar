using Microsoft.EntityFrameworkCore;

namespace SampleAPI.Entities
{
    public class SampleApiDbContext : DbContext
    {
        public SampleApiDbContext() { }
        public SampleApiDbContext(DbContextOptions<SampleApiDbContext> options) :
            base(options)
        {
        }

        public virtual DbSet<Order> Orders { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Seed data
            modelBuilder.Entity<Order>()
                .Property(o => o.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<Order>()
                .Property(o => o.Description)
                .HasMaxLength(100);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.EntryDate);
           
        }
    }
}
