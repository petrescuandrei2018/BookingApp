using Microsoft.AspNetCore.Identity;

namespace BookingApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Varsta { get; set; } // Proprietate suplimentară
        public bool TwoFactorEnabled { get; set; } = false; // Activare 2FA
    }
}
