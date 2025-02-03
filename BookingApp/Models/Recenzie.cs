using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models
{
    public class Recenzie
    {
        [Key]
        public int RecenzieId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Ratingul trebuie să fie între 1 și 5.")]
        public double Rating { get; set; }

        [Required]
        public string Descriere { get; set; }

        public string? AspectePozitive { get; set; }
        public string? AspecteNegative { get; set; }

        public bool RecomandaHotelul { get; set; }  // True dacă utilizatorul recomandă hotelul

        [Required]
        public DateTime DataRecenziei { get; set; } = DateTime.UtcNow;

        [ForeignKey("User")]
        public int UtilizatorId { get; set; }
        public User Utilizator { get; set; }

        [ForeignKey("Hotel")]
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
