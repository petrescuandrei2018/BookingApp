using BookingApp.Models;

namespace BookingApp.Services.Abstractions
{
    public interface IJwtTokenGenerator
    {
        public string GenerateToken(UserRegistered userRegistered, IEnumerable<string> roles);
    }
}
