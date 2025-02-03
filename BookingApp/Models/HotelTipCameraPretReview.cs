namespace BookingApp.Models
{
    public class HotelTipCameraPretRecenzie
    {
        public string NumeHotel { get; set; }
        public string Adresa { get; set; }
        public string NumeTipCamera { get; set; }
        public int CapacitatePersoane { get; set; }
        public int NrTotalCamere { get; set; }
        public int NrCamereDisponibile { get; set; }
        public int NrCamereOcupate { get; set; }
        public float PretNoapte { get; set; }
        public DateTime StartPretCamera { get; set; }
        public DateTime EndPretCamera { get; set; }
        public double Rating { get; set; }
        public string Descriere { get; set; }  // Înlocuire Description cu Descriere
        public string? AspectePozitive { get; set; } // Adăugăm noile câmpuri din Recenzie
        public string? AspecteNegative { get; set; }
        public bool RecomandaHotelul { get; set; }
    }
}
