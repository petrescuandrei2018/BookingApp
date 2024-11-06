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
        public DbSet<HotelTipCamera> HotelSiTipCamere { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            List<Hotel> hotels = new List<Hotel>();
            Hotel hotel1 = new Hotel(1, "Hotel1", "Brasov");
            Hotel hotel2 = new Hotel(2, "Hotel2", "Constanta");
            hotels.Add(hotel1);
            hotels.Add(hotel2);
            modelBuilder.Entity<Hotel>().HasData(hotels);

            List<TipCamera> tipCamere = new List<TipCamera>();
            TipCamera tipCameraSingle = new TipCamera(1, "Single", 1, 20, 10, 10);
            TipCamera tipCameraDouble = new TipCamera(2, "Double", 2, 40, 30, 10);
            tipCamere.Add(tipCameraSingle);
            tipCamere.Add(tipCameraDouble);
            modelBuilder.Entity<TipCamera>().HasData(tipCamere);

            modelBuilder.Entity<HotelTipCamera>()
                .HasKey(htc => new {htc.HotelId, htc.TipCameraId});

            modelBuilder.Entity<HotelTipCamera>()
                .HasOne(htc => htc.Hotel)
                .WithMany(h => h.HotelSiTipCamere)
                .HasForeignKey(htc => htc.HotelId);

            modelBuilder.Entity<HotelTipCamera>()
                .HasOne(htc => htc.TipCamera)
                .WithMany(tc => tc.HotelSiTipCamere)
                .HasForeignKey(htc => htc.TipCameraId);

            List<HotelTipCamera> hotelsTipCamere = new List<HotelTipCamera>();
            HotelTipCamera htc1 = new HotelTipCamera(1, 1);
            HotelTipCamera htc2 = new HotelTipCamera(1, 2);
            HotelTipCamera htc3 = new HotelTipCamera(2, 1);
            HotelTipCamera htc4 = new HotelTipCamera(2, 2);
            hotelsTipCamere.Add(htc1);
            hotelsTipCamere.Add(htc2);
            hotelsTipCamere.Add(htc3);
            hotelsTipCamere.Add(htc4);

            modelBuilder.Entity<HotelTipCamera>().HasData(hotelsTipCamere);
        }
    }
}
