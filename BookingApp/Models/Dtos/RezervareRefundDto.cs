namespace BookingApp.Models.Dtos
{
    public class RezervareRefundDto
    {
        public int RezervareId { get; set; }
        public int UserId { get; set; }
        public string PaymentIntentId { get; set; }
        public decimal SumaAchitata { get; set; }
        public decimal SumaTotala { get; set; }
        public decimal SumaRamasaDePlata { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string StarePlata { get; set; }
    }
}
