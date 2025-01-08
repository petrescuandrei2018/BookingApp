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

        /// <summary>
        /// Generează un token JWT pe baza ID-ului utilizatorului și a rolurilor asociate.
        /// </summary>
        public string GenereazaToken(string utilizatorId, IEnumerable<string> roluri)
        {
            var setariJwt = _configuratie.GetSection("Jwt");

            var cheieSecreta = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aVerySecureAndLongEnoughKey1234567890!"));
            if (cheieSecreta.Key.Length < 32)
            {
                throw new ArgumentException("Cheia secretă trebuie să aibă cel puțin 32 de caractere.");
            }

            var semnatura = new SigningCredentials(cheieSecreta, SecurityAlgorithms.HmacSha256);

            // Calculăm timpul curent și timpul de expirare
            var now = DateTimeOffset.UtcNow;
            var expires = now.AddMinutes(int.Parse(setariJwt["ExpiresInMinutes"]));

            var informatiiToken = new List<Claim>
            {
                new Claim("UtilizatorId", utilizatorId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            // Adăugăm rolurile utilizatorului în token
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

        /// <summary>
        /// Verifică dacă utilizatorul există și dacă parola este validă.
        /// </summary>
        public bool ValideazaUtilizator(string email, string parola)
        {
            var utilizator = _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (utilizator == null)
            {
                return false; // Utilizatorul nu există
            }

            // Verificăm parola utilizând hashing
            return BCrypt.Net.BCrypt.Verify(parola, utilizator.Password);
        }

        /// <summary>
        /// Autentifică utilizatorul și generează un token JWT.
        /// </summary>
        public string AutentificaUtilizator(string email, string parola)
        {
            if (!ValideazaUtilizator(email, parola))
            {
                return "Email sau parola incorectă.";
            }

            var utilizator = _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            // Obținem rolurile utilizatorului
            var roluri = new List<string> { utilizator.Rol.ToString() };

            return GenereazaToken(utilizator.UserId.ToString(), roluri);
        }

        /// <summary>
        /// Transformă parola utilizatorului într-un format criptat pentru securitate.
        /// </summary>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Curăță valorile string pentru a elimina caracterele problematice.
        /// </summary>
        private string CurataValoareAtribut(string valoare)
        {
            if (string.IsNullOrWhiteSpace(valoare))
                return string.Empty;

            return valoare.Trim()
                          .Replace("\r", "")
                          .Replace("\n", "")
                          .Replace("\"", "")
                          .Replace("'", "");
        }

        /// <summary>
        /// Înregistrează un utilizator nou și salvează parola într-un format criptat.
        /// </summary>
        public async Task<User> RegisterUser(UserDto userDto)
        {
            // Verificăm dacă email-ul există deja (case-insensitive)
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == userDto.Email.ToLower());
            if (existingUser != null)
            {
                throw new Exception("Email-ul este deja folosit de un alt utilizator.");
            }

            // Criptăm parola utilizatorului
            var hashedPassword = HashPassword(userDto.Password);

            // Convertim rolul în litere mici
            var rolInLitereMici = userDto.Rol.ToString().ToLower();

            // Creăm un nou utilizator
            var newUser = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email.ToLower(),
                PhoneNumber = userDto.PhoneNumber,
                Varsta = userDto.Varsta,
                Password = hashedPassword,
                Rol = Enum.Parse<RolUtilizator>(rolInLitereMici, true)
            };

            // Salvăm utilizatorul în baza de date
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser; // Returnăm utilizatorul nou creat
        }

        /// <summary>
        /// Obține un utilizator pe baza ID-ului său.
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                Console.WriteLine($"[GetUserByIdAsync] Interogare pentru userId: {userId}");
                var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                    Console.WriteLine($"[GetUserByIdAsync] Utilizatorul cu ID {userId} nu a fost găsit.");
                else
                    Console.WriteLine($"[GetUserByIdAsync] Utilizator găsit: {user.UserId}, Nume: {user.UserName}, Rol: {user.Rol}");

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUserByIdAsync] Eroare la interogare: {ex.Message}");
                throw;
            }
        }




        /// <summary>
        /// Actualizează un utilizator existent.
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                Console.WriteLine($"[UpdateUserAsync] Început metodă - userId: {user.UserId}, rol: {user.Rol}");

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[UpdateUserAsync] Modificările au fost salvate pentru utilizatorul: {user.UserId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateUserAsync] Eroare la salvarea modificărilor pentru utilizatorul: {user.UserId}. Eroare: {ex.Message}");
                return false;
            }
        }



        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

    }
}
