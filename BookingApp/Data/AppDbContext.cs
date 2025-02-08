using BookingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BookingApp.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<TipCamera> TipCamere { get; set; }
        public DbSet<PretCamera> PretCamere { get; set; }
        public DbSet<Rezervare> Rezervari { get; set; }
        public DbSet<Recenzie> Recenzii { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recenzie>()
                .Property(r => r.Rating)
                .HasColumnType("float");

            modelBuilder.Entity<Recenzie>()
                .Property(r => r.DataRecenziei)
                .HasDefaultValueSql("GETUTCDATE()");

            // Configurări pentru entitatea Rezervare
            modelBuilder.Entity<Rezervare>(entity =>
            {
                entity.Property(e => e.SumaAchitata)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.SumaRamasaDePlata)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.SumaTotala)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            // Configurări pentru entitatea ApplicationUser
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.Varsta)
                    .IsRequired();

                entity.Property(u => u.TwoFactorEnabled)
                    .HasDefaultValue(false);
            });

            // Adăugare date de test (seed)
            PrepopuleazaDate(modelBuilder);
        }

        private void PrepopuleazaDate(ModelBuilder modelBuilder)
        {
            // Seed data pentru Hotel
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel(1, "Hotel1", "Brasov", 45.6532, 25.6113),
                new Hotel(2, "Hotel2", "Constanta", 44.1598, 28.6348),
                new Hotel(3, "Hotel3", "Sibiu", 45.7983, 24.1256)
            );

            // Seed data pentru TipCamera
            modelBuilder.Entity<TipCamera>().HasData(
                new TipCamera(3, "Apartament", 4, 10, 4, 6, 1),
                new TipCamera(4, "SeaView", 2, 15, 0, 15, 2),
                new TipCamera(5, "Single", 1, 20, 4, 16, 1),
                new TipCamera(6, "Single", 1, 10, 0, 10, 2)
            );

            // Seed data pentru PretCamera
            modelBuilder.Entity<PretCamera>().HasData(
                new PretCamera(1, 900, new DateTime(2024, 10, 10), new DateTime(2024, 12, 12), 3),
                new PretCamera(2, 700, new DateTime(2024, 12, 10), new DateTime(2024, 12, 12), 4),
                new PretCamera(3, 500, new DateTime(2024, 9, 25), new DateTime(2024, 12, 12), 5),
                new PretCamera(4, 550, new DateTime(2024, 8, 9), new DateTime(2024, 12, 12), 6)
            );
        }
    }
}
