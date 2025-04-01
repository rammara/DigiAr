using Microsoft.EntityFrameworkCore;
using Mnemosyne.DataModels;

namespace Mnemosyne.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.Entity<Quote>(entity =>
            {
                entity.ToTable("quotes");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(50)")
                    .IsRequired();

                entity.Property(e => e.Price)
                    .HasPrecision(20, 4)
                    .IsRequired();

                entity.Property(e => e.TimeStamp)
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();
            });

            modelBuilder.Entity<ApiKey>(entity =>
            {
                entity.ToTable("apikeys");

                entity.Property(e => e.Key)
                    .HasColumnType("varchar(100)")
                    .IsRequired();
                entity.Property(e => e.Expires)
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();
            });
        } // void OnModelCreating
    } // class AppDbContext
} // namespace
