using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<bool> ValideazaUtilizatorAsync(string email, string parola)
    {
        var utilizator = await _userManager.FindByEmailAsync(email);
        if (utilizator == null)
            return false;

        return await _userManager.CheckPasswordAsync(utilizator, parola);
    }

    public async Task<string> AutentificaUtilizatorAsync(string email, string parola)
    {
        var utilizator = await _userManager.FindByEmailAsync(email);
        if (utilizator == null || !await _userManager.CheckPasswordAsync(utilizator, parola))
            return string.Empty;

        var roluri = await _userManager.GetRolesAsync(utilizator);
        return GenereazaToken(utilizator.Id, roluri);
    }

    public string GenereazaToken(string utilizatorId, IEnumerable<string> roluri)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, utilizatorId),
        };

        claims.AddRange(roluri.Select(rol => new Claim(ClaimTypes.Role, rol)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<ApplicationUser> RegisterUserAsync(UserDto userDto)
    {
        var utilizator = new ApplicationUser
        {
            UserName = userDto.UserName,
            Email = userDto.Email,
            PhoneNumber = userDto.PhoneNumber,
            Varsta = userDto.Varsta // Adăugăm vârsta utilizatorului
        };

        var result = await _userManager.CreateAsync(utilizator, userDto.Password);
        if (!result.Succeeded)
            throw new Exception("Eroare la înregistrare.");

        return utilizator;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        var utilizator = await _userManager.FindByIdAsync(user.UserId.ToString());
        if (utilizator == null)
            return false;

        utilizator.Email = user.Email;
        utilizator.PhoneNumber = user.PhoneNumber;

        var result = await _userManager.UpdateAsync(utilizator);
        return result.Succeeded;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();
        return users.Cast<User>().ToList();
    }

    public async Task<bool> ExistaEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    public async Task<bool> ExistaTelefonAsync(string phoneNumber)
    {
        return await _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
    }

    // 🔹 2FA
    public async Task<string?> GenereazaCheie2FAAsync(string userId)
    {
        var utilizator = await _userManager.FindByIdAsync(userId);
        if (utilizator == null)
            return null;

        var key = await _userManager.GetAuthenticatorKeyAsync(utilizator);
        if (string.IsNullOrEmpty(key))
        {
            await _userManager.ResetAuthenticatorKeyAsync(utilizator);
            key = await _userManager.GetAuthenticatorKeyAsync(utilizator);
        }

        return key;
    }

    public async Task<bool> Verifica2FAAsync(string userId, string codTOTP)
    {
        var utilizator = await _userManager.FindByIdAsync(userId);
        if (utilizator == null)
            return false;

        return await _userManager.VerifyTwoFactorTokenAsync(utilizator, TokenOptions.DefaultAuthenticatorProvider, codTOTP);
    }
}
