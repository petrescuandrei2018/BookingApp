using BookingApp.Models.Dtos;
using BookingApp.Models;

public interface IAuthService
{
    string GenereazaToken(string utilizatorId, IEnumerable<string> roluri);
    bool ValideazaUtilizator(string email, string parola);
    string AutentificaUtilizator(string email, string parola);
    Task<User> RegisterUser(UserDto userDto);
    Task<User?> GetUserByIdAsync(int userId);
    Task<bool> UpdateUserAsync(User user);
    Task<List<User>> GetAllUsersAsync();

    // Metode pentru validarea existenței
    Task<bool> ExistaEmailAsync(string email);
    Task<bool> ExistaTelefonAsync(string phoneNumber);
}
