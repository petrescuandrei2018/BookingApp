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
            // Normalizează email-ul înainte de query
            email = email.ToLower();

            var utilizator = _context.Users
                .FirstOrDefault(u => u.Email == email);

            if (utilizator == null)
            {
                return false;
            }

            // Verifică parola criptată
            return BCrypt.Net.BCrypt.Verify(parola, utilizator.Password);
        }

        public async Task<bool> ExistaEmailAsync(string email)
        {
            // Normalizează email-ul înainte de query
            email = email.ToLower();

            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistaTelefonAsync(string phoneNumber)
        {
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User> RegisterUser(UserDto userDto)
        {
            // Verifică dacă email-ul și numărul de telefon sunt unice
            if (await ExistaEmailAsync(userDto.Email))
            {
                throw new Exception("Email-ul este deja utilizat.");
            }

            if (await ExistaTelefonAsync(userDto.PhoneNumber))
            {
                throw new Exception("Numărul de telefon este deja utilizat.");
            }

            // Normalizează email-ul și criptează parola
            var user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email.ToLower(), // Normalizare email
                PhoneNumber = userDto.PhoneNumber,
                Varsta = userDto.Varsta,
                Password = HashPassword(userDto.Password), // Criptare parolă
                Rol = userDto.Rol // Preia rolul din DTO
            };

            // Salvează utilizatorul în baza de date
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public string AutentificaUtilizator(string email, string parola)
        {
            // Normalizează email-ul înainte de validare
            email = email.ToLower();

            if (!ValideazaUtilizator(email, parola))
            {
                return "Email sau parola incorectă.";
            }

            var utilizator = _context.Users.FirstOrDefault(u => u.Email == email);
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
    }
}
