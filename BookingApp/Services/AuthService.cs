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

        /// Generează un token JWT pe baza utilizatorului
        public string GenereazaToken(string utilizatorId, IEnumerable<string> roluri)
        {
            var setariJwt = _configuratie.GetSection("Jwt");

            var cheieSecreta = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aVerySecureAndLongEnoughKey1234567890!"));

            if (cheieSecreta.Key.Length < 32)
            {
                throw new ArgumentException("Cheia secretă trebuie să aibă cel puțin 32 de caractere.");
            }

            var semnatura = new SigningCredentials(cheieSecreta, SecurityAlgorithms.HmacSha256);

            // Calculăm timpul curent și timpul de expirare ca timestamp-uri UNIX
            var now = DateTimeOffset.UtcNow;
            var expires = now.AddMinutes(int.Parse(setariJwt["ExpiresInMinutes"]));

            var informatiiToken = new List<Claim>
    {
        new Claim("UtilizatorId", utilizatorId),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
    };

            // Adăugăm rolurile în token
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

            // În exemplu, presupunem că utilizatorul are un rol definit ca "User"
            var roluri = new List<string> { "User" };

            return GenereazaToken(utilizator.UserId.ToString(), roluri);
        }

        /// Transformă parola utilizatorului într-un format criptat pentru a asigura securitatea acesteia în baza de date
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// Curăță valorile atributelor pentru a elimina caractere problematice
        private string CurataValoareAtribut(string valoare)
        {
            if (string.IsNullOrWhiteSpace(valoare))
                return string.Empty;

            valoare = valoare.Trim();
            valoare = valoare.Replace("\r", "").Replace("\n", "").Replace("\"", "").Replace("'", "");
            return valoare;
        }

        /// Înregistrează un utilizator nou și salvează parola într-un format criptat
        public async Task<User> RegisterUser(UserDto userDto)
        {
            // Verificăm dacă email-ul există deja în baza de date (case-insensitive)
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == userDto.Email.ToLower());
            if (existingUser != null)
            {
                throw new Exception("Email-ul este deja folosit de un alt utilizator.");
            }

            // Criptăm parola utilizatorului
            var hashedPassword = HashPassword(userDto.Password);

            // Convertim valoarea enum "Rol" în text cu litere mici
            var rolInLitereMici = userDto.Rol.ToString().ToLower();

            // Creăm obiectul User pentru salvare
            var newUser = new User
            {
                UserName = userDto.UserName, // Numele utilizatorului
                Email = userDto.Email.ToLower(), // Email salvat în litere mici
                PhoneNumber = userDto.PhoneNumber, // Numărul de telefon
                Varsta = userDto.Varsta, // Vârsta utilizatorului
                Password = hashedPassword, // Parola criptată
                Rol = Enum.Parse<RolUtilizator>(rolInLitereMici, true) // Conversie finală în enum
            };

            // Salvăm utilizatorul în baza de date
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser; // Returnăm utilizatorul nou creat
        }
    }
}
