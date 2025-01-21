using BookingApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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

    public string Stare { get; set; }
    public string StarePlata { get; set; } = "Neplatita";
    public string ClientSecret { get; set; }

    public decimal SumaTotala { get; set; }
    public decimal SumaAchitata { get; set; }
    public decimal SumaRamasaDePlata { get; set; }

    [NotMapped]
    public string HotelName => PretCamera?.TipCamera?.Hotel?.Name ?? "Necunoscut";

    public void CalculareSumaTotala()
    {
        if (PretCamera == null)
        {
            throw new InvalidOperationException("PretCamera nu este setat.");
        }

        int numarNopti = (int)(CheckOut - CheckIn).TotalDays;
        if (numarNopti <= 0)
        {
            throw new InvalidOperationException("Intervalul CheckIn-CheckOut nu este valid.");
        }

        SumaTotala = numarNopti * (decimal)PretCamera.PretNoapte;
        SumaRamasaDePlata = SumaTotala - SumaAchitata;
    }
}
