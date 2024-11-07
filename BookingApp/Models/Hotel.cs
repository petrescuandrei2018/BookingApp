using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models
{
    public class Hotel : BaseEntity
    {
        [Key]
        public int HotelId { get; set; }
        public string Name { get; set; }
        public string Address {  get; set; }

        public Hotel(int hotelId, string name, string address)
        {
            HotelId = hotelId;
            Name = name;
            Address = address;
        }
    }
}
