using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public double Rating { get; set; }
        public string Description { get; set; }
        [ForeignKey("Hotel")]
        public int HotelId {  get; set; }
        public Hotel Hotel { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }

        public Review(int reviewId, double rating, string description, int hotelId, int userId)
        {
            ReviewId = reviewId;
            Rating = rating;
            Description = description;
            HotelId = hotelId;
            UserId = userId;
        }
    }
}
