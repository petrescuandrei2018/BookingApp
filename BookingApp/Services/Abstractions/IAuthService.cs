using BookingApp.Models.Dtos;

namespace BookingApp.Services.Abstractions
{
    public interface IAuthService
    {

            /// Generează un token JWT pe baza utilizatorului și a rolurilor
            string GenereazaToken(string utilizatorId, IEnumerable<string> roluri);

            /// Validează dacă utilizatorul există și dacă datele sunt corecte
            bool ValideazaUtilizator(string email, string parola);
            
            /// Autentifică utilizatorul și returnează un token JWT
            string AutentificaUtilizator(string email, string parola);
            Task<bool> RegisterUser(UserDto userDto);

    }
}
