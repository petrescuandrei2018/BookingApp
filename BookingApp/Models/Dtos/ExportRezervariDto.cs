namespace BookingApp.Models.Dtos
{
    public class ExportRezervariDto
    {
        public int RezervareId { get; set; }
        public string NumeUtilizator { get; set; }
        public string Email { get; set; }
        public string NumeHotel { get; set; }
        public string TipCamera { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal SumaAchitata { get; set; }
        public string StarePlata { get; set; }
    }
}
