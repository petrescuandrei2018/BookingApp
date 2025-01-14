using System;
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

        public string StarePlata { get; set; } = "Neplatita"; // Valori posibile: Neplatita, Platita, Esuata

        public string ClientSecret { get; set; } // Secret Stripe pentru plată

        public decimal SumaTotala { get; set; }
        public decimal SumaAchitata { get; set; }
        public decimal SumaRamasaDePlata { get; set; }


        // Metoda pentru calcularea sumei totale
        public void CalculareSumaTotala()
        {
            if (PretCamera == null)
            {
                throw new InvalidOperationException("PretCamera nu este setat.");
            }

            // Calculăm numărul de nopți
            int numarNopti = (int)(CheckOut - CheckIn).TotalDays;
            if (numarNopti <= 0)
            {
                throw new InvalidOperationException("Intervalul CheckIn-CheckOut nu este valid.");
            }

            // Calculăm suma totală
            SumaTotala = numarNopti * (decimal)PretCamera.PretNoapte;
        }
    }
}
