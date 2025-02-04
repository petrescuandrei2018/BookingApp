namespace BookingApp.Models.Dtos
{
    public class CreareRezervareDto
    {
        public int UserId { get; set; }
        public int HotelId { get; set; }
        public int TipCameraId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
