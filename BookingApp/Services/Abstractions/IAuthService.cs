using System.Collections.Generic;
using System.Threading.Tasks;
using BookingApp.Models.Dtos;
using BookingApp.Models;

public interface IAuthService
{
    // ✅ Validare utilizator
    Task<bool> ValideazaUtilizatorAsync(string email, string parola);

    // ✅ Autentificare
    Task<string> AutentificaUtilizatorAsync(string email, string parola);
    Task<ApplicationUser> GetUserByIdAsync(string userId);

    // ✅ Înregistrare
    Task<(ApplicationUser utilizator, string rol)> RegisterUserAsync(UserDto userDto);

    // ✅ Gestionare utilizatori
    Task<bool> UpdateUserAsync(ApplicationUser user);
    Task<List<UserDto>> GetAllUsersAsync();
    Task<bool> ExistaEmailAsync(string email);
    Task<bool> ExistaTelefonAsync(string phoneNumber);

    // ✅ Gestionare roluri
    Task<bool> AtribuieRolAsync(string userId, string rol);
    Task<List<string>> GetUserRolesAsync(string userId);

    // ✅ Two-Factor Authentication (2FA)
    Task<string?> GenereazaCheie2FAAsync(string userId);
    Task<bool> Verifica2FAAsync(string userId, string codTOTP);
    Task<bool> Activeaza2FAAsync(string userId);
    Task<bool> Dezactiveaza2FAAsync(string userId);
}
