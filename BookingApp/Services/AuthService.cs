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

        /// Generează un token JWT pe baza utilizatorului și a rolurilor
        public string GenereazaToken(string utilizatorId)
        {
            Console.WriteLine("Începem generarea token-ului...");

            var setariJwt = _configuratie.GetSection("Jwt");
            Console.WriteLine($"Issuer: {setariJwt["Issuer"]}");
            Console.WriteLine($"Audience: {setariJwt["Audience"]}");
            Console.WriteLine($"ExpiresInMinutes: {setariJwt["ExpiresInMinutes"]}");

            var cheieSecreta = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aVerySecureAndLongEnoughKey1234567890!"));

            if (cheieSecreta.Key.Length < 32)
            {
                Console.WriteLine("Eroare: Cheia secretă are mai puțin de 32 de caractere!");
                throw new ArgumentException("Cheia secretă trebuie să aibă cel puțin 32 de caractere.");
            }
            Console.WriteLine("Cheia secretă este validă.");

            var semnatura = new SigningCredentials(cheieSecreta, SecurityAlgorithms.HmacSha256);
            Console.WriteLine("Semnătura generată cu succes.");

            // Calculăm timpul curent și timpul de expirare ca timestamp-uri UNIX
            var now = DateTimeOffset.UtcNow;
            var expires = now.AddMinutes(int.Parse(setariJwt["ExpiresInMinutes"]));

            var informatiiToken = new List<Claim>
            {
                new Claim("UtilizatorId", utilizatorId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            Console.WriteLine($"Revendicare: iat = {now.ToUnixTimeSeconds()}");
            Console.WriteLine($"Revendicare: exp = {expires.ToUnixTimeSeconds()}");

            var token = new JwtSecurityToken(
                issuer: setariJwt["Issuer"],
                audience: setariJwt["Audience"],
                claims: informatiiToken,
                expires: expires.UtcDateTime,
                signingCredentials: semnatura
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            Console.WriteLine($"Token generat cu succes: {tokenString}");
            return tokenString;
        }

        /// Validează credențialele utilizatorului folosind hashing pentru parola
        public bool ValideazaUtilizator(string email, string parola)
        {
            var utilizator = _context.Users.FirstOrDefault(u => u.Email == email);

            if (utilizator == null)
            {
                return false; // Utilizatorul nu există
            }

            // Verificăm parola folosind hashing
            return BCrypt.Net.BCrypt.Verify(parola, utilizator.Password);
        }

        /// Autentifică utilizatorul și returnează un token JWT
        public string AutentificaUtilizator(string email, string parola)
        {
            if (ValideazaUtilizator(email, parola) == false)
            {
                return "Email sau parola incorectă.";
            }

            var utilizator = _context.Users.FirstOrDefault(u => u.Email == email);

            return GenereazaToken(utilizator.UserId.ToString());
        }

        /// Transformă parola utilizatorului într-un format criptat pentru a asigura securitatea acesteia în baza de date
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private string CurataValoareRevendicare(string valoare)
        {
            if (string.IsNullOrWhiteSpace(valoare))
                return string.Empty;

            // Eliminăm spațiile și caracterele speciale care pot cauza probleme
            valoare = valoare.Trim();
            valoare = valoare.Replace("\r", "").Replace("\n", "").Replace("\"", "").Replace("'", "");
            return valoare;
        }

        /// Înregistrează un utilizator nou și salvează parola într-un format criptat
        public async Task<User> RegisterUser(UserDto userDto)
        {
            // Verificăm dacă email-ul există deja în baza de date
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser != null)
            {
                throw new Exception("Email-ul este deja folosit de un alt utilizator.");
            }

            // Criptăm parola utilizatorului
            var hashedPassword = HashPassword(userDto.Password);

            // Creăm obiectul User
            var newUser = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                Varsta = userDto.Varsta,
                Password = hashedPassword
            };

            // Salvăm utilizatorul în baza de date
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }
    }
}