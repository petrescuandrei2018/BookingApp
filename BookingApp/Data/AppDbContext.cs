using BookingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BookingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<TipCamera> TipCamere { get; set; }
        public DbSet<PretCamera> PretCamere { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Rezervare> Rezervari { get; set; }

        // Adăugăm această metodă
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            // Configurări pentru entitatea User
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Rol)
                    .IsRequired()
                    .HasDefaultValue("user") // Valoare implicită
                    .HasMaxLength(20); // Lungime maximă
            });

            // Date seed
            PrepopuleazaDate(modelBuilder);
        }

        private void PrepopuleazaDate(ModelBuilder modelBuilder)
        {
            // Seed data
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel(1, "Hotel1", "Brasov"),
                new Hotel(2, "Hotel2", "Constanta"),
                new Hotel(3, "Hotel3", "Sibu")
            );

            modelBuilder.Entity<TipCamera>().HasData(
                new TipCamera(3, "Apartament", 4, 10, 4, 6, 1),
                new TipCamera(4, "SeaView", 2, 15, 0, 15, 2),
                new TipCamera(5, "Single", 1, 20, 4, 16, 1),
                new TipCamera(6, "Single", 1, 10, 0, 10, 2)
            );

            modelBuilder.Entity<PretCamera>().HasData(
                new PretCamera(1, 900, new DateTime(2024, 10, 10), new DateTime(2024, 12, 12), 3),
                new PretCamera(2, 700, new DateTime(2024, 12, 10), new DateTime(2024, 12, 12), 4),
                new PretCamera(3, 500, new DateTime(2024, 9, 25), new DateTime(2024, 12, 12), 5),
                new PretCamera(4, 550, new DateTime(2024, 8, 9), new DateTime(2024, 12, 12), 6)
            );

            modelBuilder.Entity<User>().HasData(
                new User(1, "Mihai", "mihai@gmail.com", "0775695878", 30, BCrypt.Net.BCrypt.HashPassword("parola1")) { Rol = "admin" },
                new User(2, "Nicu", "nicu@gmail.com", "0770605078", 20, BCrypt.Net.BCrypt.HashPassword("parola2")) { Rol = "admin" },
                new User(3, "Alex", "alex@gmail.com", "0765665668", 32, BCrypt.Net.BCrypt.HashPassword("parola3")) { Rol = "user" }
            );

            modelBuilder.Entity<Review>().HasData(
                new Review(1, 4.9, "A fost bine", 1, 1),
                new Review(2, 5, "Mancare excelenta", 1, 2),
                new Review(3, 5, "Priveliste la mare", 1, 3),
                new Review(4, 3.5, "Cazare tarzie", 1, 3),
                new Review(5, 2, "Muste in camera", 2, 2)
            );
        }
    }
}
