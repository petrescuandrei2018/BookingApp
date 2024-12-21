using Microsoft.AspNetCore.Identity;

namespace BookingApp.Models
{
    public class UserRegistered : IdentityUser
    {
        public string Name { get; set; }

    }
}
