using BookingApp.Models;
using BookingApp.Services.Abstractions;
using Microsoft.Extensions.Options;

namespace BookingApp.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwt;
        public JwtTokenGenerator(IOptions<JwtOptions> jwt)
        {
            _jwt = jwt.Value;
        }
        public string GenerateToken(UserRegistered userRegistered, IEnumerable<string> roles)
        {
            throw new NotImplementedException();
        }
    }
}
