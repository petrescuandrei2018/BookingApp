namespace BookingApp.Models
{
    public class HotelTipCamera
    {
        public string HotelName { get; set; }
        public string Address { get; set; }
        public string TipCameraName { get; set; }
        public int CapacitatePersoane { get; set; }
        public int NrTotalCamere { get; set; }
        public int NrCamereDisponibile { get; set; }
        public int NrCamereOcupate { get; set; }
        public int TipCameraId { get; set; }
    }
}
