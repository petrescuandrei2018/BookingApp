using System.Collections.Generic;

public class HotelHartaDto
{
    public int HotelId { get; set; }
    public string Nume { get; set; }
    public string Adresa { get; set; }
    public double Latitudine { get; set; }
    public double Longitudine { get; set; }
    public List<string> TipuriCamere { get; set; } = new();
    public decimal PretMediuNoapte { get; set; }
    public double RatingMediu { get; set; }
    public string ImagineUrl { get; set; }
}
