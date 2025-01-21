using System.Text.Json.Serialization;

public class RezervareDto
{
    [JsonIgnore]
    public int RezervareId { get; set; } // Generat automat

    [JsonIgnore]
    public int UserId { get; set; } // Preluat din JWT

    public string HotelName { get; set; } // Setat automat în backend
    public string NumeCamera { get; set; } // Tipul camerei
    public DateTime CheckIn { get; set; } // Data Check-In
    public DateTime CheckOut { get; set; } // Data Check-Out

    [JsonIgnore]
    public decimal Pret { get; set; } // Calculat automat

    [JsonIgnore]
    public string Stare { get; set; } = "Viitoare"; // Implicit "Viitoare"
}
