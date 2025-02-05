using BookingApp.Models.Dtos;
using BookingApp.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

public interface IAuthService
{
    string GenereazaToken(string utilizatorId, IEnumerable<string> roluri);
    Task<bool> ValideazaUtilizatorAsync(string email, string parola);
    Task<string> AutentificaUtilizatorAsync(string email, string parola);
    Task<User> RegisterUserAsync(UserDto userDto);
    Task<User?> GetUserByIdAsync(int userId);
    Task<bool> UpdateUserAsync(User user);
    Task<List<User>> GetAllUsersAsync();

    // Validare existență
    Task<bool> ExistaEmailAsync(string email);
    Task<bool> ExistaTelefonAsync(string phoneNumber);

    // 🔹 2FA
    Task<string?> GenereazaCheie2FAAsync(string userId);
    Task<bool> Verifica2FAAsync(string userId, string codTOTP);
}
