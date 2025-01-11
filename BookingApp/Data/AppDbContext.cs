using BookingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<TipCamera> TipCamere { get; set; }
        public DbSet<PretCamera> PretCamere { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Rezervare> Rezervari { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurare pentru entitatea User
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            // Configurare pentru entitatea Rezervare
            modelBuilder.Entity<Rezervare>(configRezervare =>
            {
                configRezervare.Property(r => r.Stare)
                    .HasConversion<string>()
                    .IsRequired();

                configRezervare.Property(r => r.StarePlata)
                    .HasDefaultValue("Neplatita") // Valoare implicită pentru `StarePlata`
                    .IsRequired(); // Setăm câmpul ca fiind obligatoriu
            });

            // Configurare date prepopulate (se păstrează logica existentă)
            Hotel hotel1 = new Hotel(1, "Hotel1", "Brasov")
            {
                Address = "Brasov",
                HotelId = 1,
                Name = "Hotel1"
            };

            Hotel hotel2 = new Hotel(2, "Hotel2", "Constanta")
            {
                Address = "Constanta",
                HotelId = 2,
                Name = "Hotel2"
            };

            modelBuilder.Entity<Hotel>().HasData(hotel1, hotel2);

            List<TipCamera> tipCameras = new List<TipCamera>
            {
                new TipCamera(1, "Single", 1, 20, 10, 10, 1),
                new TipCamera(2, "Double", 2, 40, 30, 10, 2),
                new TipCamera(3, "Apartament", 4, 10, 1, 9, 1),
                new TipCamera(4, "SeaView", 2, 15, 2, 13, 2),
                new TipCamera(5, "Single", 1, 20, 10, 10, 1),
                new TipCamera(6, "Single", 1, 10, 5, 5, 2)
            };

            modelBuilder.Entity<TipCamera>().HasData(tipCameras);

            List<PretCamera> pretCamere = new List<PretCamera>
            {
                new PretCamera(1, 900, new DateTime(2024, 10, 10), new DateTime(2024, 12, 12), 3),
                new PretCamera(2, 700, new DateTime(2024, 12, 10), new DateTime(2024, 12, 12), 4),
                new PretCamera(3, 500, new DateTime(2024, 09, 25), new DateTime(2024, 12, 12), 5),
                new PretCamera(4, 550, new DateTime(2024, 08, 09), new DateTime(2024, 12, 12), 6)
            };

            modelBuilder.Entity<PretCamera>().HasData(pretCamere);

            List<User> users = new List<User>
            {
                new User(1, "Mihai", "mihai@gmail.com", "0775695878", 30, "parola1"),
                new User(2, "Nicu", "nicu@gmail.com", "0770605078", 20, "parola2"),
                new User(3, "Alex", "alex@gmail.com", "0765665668", 32, "parola3")
            };

            modelBuilder.Entity<User>().HasData(users);

            List<Review> reviews = new List<Review>
            {
                new Review(1, 4.9, "A fost bine", 1, 1),
                new Review(2, 5, "Mancare excelenta", 1, 2),
                new Review(3, 5, "Priveliste la mare", 1, 3),
                new Review(4, 3.5, "Cazare tarzie", 1, 3),
                new Review(5, 2, "Muste in camera", 2, 2)
            };

            modelBuilder.Entity<Review>().HasData(reviews);

            Hotel h3 = new Hotel(3, "Hotel3", "Sibu");
            modelBuilder.Entity<Hotel>().HasData(h3);
        }
    }
}
