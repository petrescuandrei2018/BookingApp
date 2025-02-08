namespace BookingApp.Models.Dtos
{
    public class UpdateUserDto
    {
        public string UserId { get; set; }  // IdentityUser folosește string pentru ID
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }  // Pentru activarea/dezactivarea 2FA
    }
}
