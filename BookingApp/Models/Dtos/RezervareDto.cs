using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models.Dtos
{
    public class RezervareDto
    {
        public int UserId { get; set; }
        public int PretCameraId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
