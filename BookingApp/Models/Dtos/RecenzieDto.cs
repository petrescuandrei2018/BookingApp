namespace BookingApp.Models.Dtos
{
    public class RecenzieDto
    {
        public int HotelId { get; set; }
        public int UtilizatorId { get; set; }
        public double Rating { get; set; }
        public string Descriere { get; set; }
        public string? AspectePozitive { get; set; }
        public string? AspecteNegative { get; set; }
        public bool RecomandaHotelul { get; set; }
    }
}
