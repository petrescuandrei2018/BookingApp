namespace BookingApp.Models.Dtos
{
    public class DashboardStatisticiDto
    {
        public int RezervariActive { get; set; }
        public decimal VenitZi { get; set; }
        public decimal VenitSaptamana { get; set; }
        public decimal VenitLuna { get; set; }
        public List<HotelRezervariDto> HoteluriFrecvente { get; set; }
        public int UtilizatoriActivi { get; set; }
        public int UtilizatoriInactivi { get; set; }
    }
}
