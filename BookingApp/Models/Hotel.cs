using System.ComponentModel.DataAnnotations;

namespace BookingApp.Models
{
    public class Hotel : BaseEntity
    {
        [Key]
        public int HotelId { get; set; }
        public string Name { get; set; }
        public string Address {  get; set; }
        public ICollection<HotelTipCamera> HotelSiTipCamere { get; set; }

        public Hotel(int hotelId, string name, string address)
        {
            HotelId = hotelId;
            Name = name;
            Address = address;
        }
    }
}
