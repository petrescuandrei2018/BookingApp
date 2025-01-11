using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models
{
    public class Rezervare : BaseEntity
    {
        [Key]
        public int RezervareId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("PretCamera")]
        public int PretCameraId { get; set; }
        public PretCamera PretCamera { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public StareRezervare Stare { get; set; }

        // Nou câmp pentru statusul plății
        public string StarePlata { get; set; } = "Neplatita"; // Valori posibile: Neplatita, Platita, Esuata

        // Constructor cu parametrul implicit pentru statusul plății
        public Rezervare(int rezervareId, int userId, int pretCameraId, DateTime checkIn, DateTime checkOut, string starePlata = "Neplatita")
        {
            RezervareId = rezervareId;
            UserId = userId;
            PretCameraId = pretCameraId;
            CheckIn = checkIn;
            CheckOut = checkOut;
            StarePlata = starePlata;
        }

        public Rezervare() { }
    }
}
