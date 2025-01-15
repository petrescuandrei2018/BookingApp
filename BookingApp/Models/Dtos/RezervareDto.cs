using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models.Dtos
{
    public class RezervareDto
    {
        public int RezervareId { get; set; } // Adăugat
        public int UserId { get; set; }
        public int PretCameraId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal Pret { get; set; } // Adăugat
        public string HotelName { get; set; } // Adăugat
        public string Stare { get; set; } // Adăugat
    }
}
