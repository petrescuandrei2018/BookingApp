namespace BookingApp.Models.Dtos
{
    public class NonExpiredRezervareDto
    {
        public int RezervareId { get; set; }
        public int UserId { get; set; }
        public string HotelName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal Pret { get; set; }
        public string Stare { get; set; }
    }
}
