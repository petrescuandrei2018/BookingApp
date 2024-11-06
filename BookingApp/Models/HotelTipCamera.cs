namespace BookingApp.Models
{
    public class HotelTipCamera
    {
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
        public int TipCameraId { get; set; }
        public TipCamera TipCamera { get; set; }

        public HotelTipCamera(int hotelId, int tipCameraId)
        {
            HotelId = hotelId;
            TipCameraId = tipCameraId;
        }
    }
}
