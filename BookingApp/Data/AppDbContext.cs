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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
            modelBuilder.Entity<Hotel>().HasData(hotel1);
            modelBuilder.Entity<Hotel>().HasData(hotel2);


            /*List<Hotel> hotels = new List<Hotel>();
            Hotel hotel1 = new Hotel(1, "Hotel1", "Brasov");
            Hotel hotel2 = new Hotel(2, "Hotel2", "Constanta");
            hotels.Add(hotel1);
            hotels.Add(hotel2);
            modelBuilder.Entity<Hotel>().HasData(hotels);*/

            /*List<TipCamera> tipCamere = new List<TipCamera>();
            TipCamera tipCameraSingle = new TipCamera(1, "Single", 1, 20, 10, 10, 1);
            TipCamera tipCameraDouble = new TipCamera(2, "Double", 2, 40, 30, 10, 2);
            tipCamere.Add(tipCameraSingle);
            tipCamere.Add(tipCameraDouble);
            modelBuilder.Entity<TipCamera>().HasData(tipCamere);*/

            List<TipCamera> tipCameras = new List<TipCamera>();
            TipCamera tipCamera1 = new TipCamera(1, "Single", 1, 20, 10, 10, 1);
            TipCamera tipCamera2 = new TipCamera(2, "Double", 2, 40, 30, 10, 2);
            tipCameras.Add(tipCamera1);
            tipCameras.Add(tipCamera2);
            modelBuilder.Entity<TipCamera>().HasData(tipCameras);

            TipCamera tipCameraApartament = new TipCamera(3, "Apartament", 4, 10, 1, 9, 1);
            modelBuilder.Entity<TipCamera>().HasData(tipCameraApartament);

            TipCamera tipCameraSeaView = new TipCamera(4, "SeaView", 2, 15, 2, 13, 2);
            modelBuilder.Entity<TipCamera>().HasData(tipCameraSeaView);

            TipCamera tipCameraSingle = new TipCamera(5, "Single", 1, 20, 10, 10, 1);
            modelBuilder.Entity<TipCamera>().HasData(tipCameraSingle);

            TipCamera tipCameraSingle2 = new TipCamera(6, "Single", 1, 10, 5, 5, 2);
            modelBuilder.Entity<TipCamera>().HasData(tipCameraSingle2);

            PretCamera pretCameraApartament = new PretCamera(1, 900, new DateTime(2024,10,10), new DateTime(2024,12,12), 3);
            PretCamera pretCameraSeaView = new PretCamera(2, 700, new DateTime(2024,12, 10), new DateTime(2024, 12, 12), 4);
            PretCamera pretCameraSingle = new PretCamera(3, 500, new DateTime(2024, 09, 25), new DateTime(2024, 12, 12), 5);
            PretCamera pretCameraSingle2 = new PretCamera(4, 550, new DateTime(2024, 08, 09), new DateTime(2024, 12, 12), 6);
            modelBuilder.Entity<PretCamera>().HasData(pretCameraApartament);
            modelBuilder.Entity<PretCamera>().HasData(pretCameraSeaView);
            modelBuilder.Entity<PretCamera>().HasData(pretCameraSingle);
            modelBuilder.Entity<PretCamera>().HasData(pretCameraSingle2);

        }
    }
}
