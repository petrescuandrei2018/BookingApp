using BCrypt.Net;
using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuratie;
        private readonly AppDbContext _context;

        public AuthService(IConfiguration configuratie, AppDbContext context)
        {
            _configuratie = configuratie;
            _context = context;
        }

        public string GenereazaToken(string utilizatorId, IEnumerable<string> roluri)
        {
            var setariJwt = _configuratie.GetSection("Jwt");

            var cheieSecreta = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aVerySecureAndLongEnoughKey1234567890!"));
            if (cheieSecreta.Key.Length < 32)
            {
                throw new ArgumentException("Cheia secretă trebuie să aibă cel puțin 32 de caractere.");
            }

            var semnatura = new SigningCredentials(cheieSecreta, SecurityAlgorithms.HmacSha256);

            var now = DateTimeOffset.UtcNow;
            var expires = now.AddMinutes(int.Parse(setariJwt["ExpiresInMinutes"]));

            var informatiiToken = new List<Claim>
            {
                new Claim("UtilizatorId", utilizatorId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            informatiiToken.AddRange(roluri.Select(rol => new Claim(ClaimTypes.Role, rol)));

            var token = new JwtSecurityToken(
                issuer: setariJwt["Issuer"],
                audience: setariJwt["Audience"],
                claims: informatiiToken,
                expires: expires.UtcDateTime,
                signingCredentials: semnatura
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValideazaUtilizator(string email, string parola)
        {
            var utilizator = _context.Users.FirstOrDefault(u => u.Email.ToLowerInvariant() == email.ToLowerInvariant());
            if (utilizator == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(parola, utilizator.Password);
        }

        public async Task<User> RegisterUser(UserDto userDto)
        {
            var emailLower = userDto.Email?.ToLowerInvariant();
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLowerInvariant() == emailLower);
            if (existingUser != null)
            {
                throw new Exception("Email-ul este deja folosit de un alt utilizator.");
            }

            var hashedPassword = HashPassword(userDto.Password);

            var rolLower = userDto.Rol?.ToLowerInvariant();
            if (rolLower != "admin" && rolLower != "user")
            {
                throw new Exception("Rol invalid. Permis doar 'admin' sau 'user'.");
            }

            var newUser = new User
            {
                UserName = userDto.UserName,
                Email = emailLower,
                PhoneNumber = userDto.PhoneNumber,
                Varsta = userDto.Varsta,
                Password = hashedPassword,
                Rol = rolLower
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public string AutentificaUtilizator(string email, string parola)
        {
            if (!ValideazaUtilizator(email, parola))
            {
                return "Email sau parola incorectă.";
            }

            var utilizator = _context.Users.FirstOrDefault(u => u.Email.ToLowerInvariant() == email.ToLowerInvariant());
            var roluri = new List<string> { utilizator.Rol };

            return GenereazaToken(utilizator.UserId.ToString(), roluri);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                // Validare pentru `Rol` dacă este necesară
                if (user.Rol != "admin" && user.Rol != "user")
                {
                    throw new Exception("Rol invalid. Permis doar 'admin' sau 'user'.");
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
