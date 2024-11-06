namespace BookingApp.Models
{
    public abstract class BaseEntity
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
