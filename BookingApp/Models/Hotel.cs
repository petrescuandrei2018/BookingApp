using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models
{
    public class Hotel : BaseEntity
    {
        [Key]
        public int HotelId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Latitudine { get; set; } // Latitudine geografică
        public double Longitudine { get; set; } // Longitudine geografică

        public Hotel(int hotelId, string name, string address, double latitudine, double longitudine)
        {
            HotelId = hotelId;
            Name = name;
            Address = address;
            Latitudine = latitudine;
            Longitudine = longitudine;
        }
    }
}
