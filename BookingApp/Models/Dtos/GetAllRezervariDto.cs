namespace BookingApp.Models.Dtos
{
    public class GetAllRezervariDto
    {
        public int UserId { get; set; }
        public string HotelName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal Pret { get; set; }
        public string Stare { get; set; }
    }
}
