using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
    }

    // ✅ Validare utilizator
    public async Task<bool> ValideazaUtilizatorAsync(string email, string parola)
    {
        var utilizator = await _userManager.FindByEmailAsync(email);
        if (utilizator == null)
            return false;

        return await _userManager.CheckPasswordAsync(utilizator, parola);
    }

    // ✅ Autentificare și generare token JWT
    public async Task<string> AutentificaUtilizatorAsync(string email, string parola)
    {
        var utilizator = await _userManager.FindByEmailAsync(email);
        if (utilizator == null || !await _userManager.CheckPasswordAsync(utilizator, parola))
            return string.Empty;

        var roluri = await _userManager.GetRolesAsync(utilizator);
        return GenereazaToken(utilizator.Id, roluri);
    }

    private string GenereazaToken(string utilizatorId, IEnumerable<string> roluri)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, utilizatorId)
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

    // ✅ Înregistrare utilizator
    public async Task<(ApplicationUser utilizator, string rol)> RegisterUserAsync(UserDto userDto)
    {
        var utilizator = new ApplicationUser
        {
            UserName = userDto.UserName,
            Email = userDto.Email,
            PhoneNumber = userDto.PhoneNumber,
            TwoFactorEnabled = false
        };

        var rezultat = await _userManager.CreateAsync(utilizator, userDto.Password);
        if (!rezultat.Succeeded)
            throw new Exception("Eroare la înregistrare: " + string.Join(", ", rezultat.Errors.Select(e => e.Description)));

        // Setăm rolul utilizatorului
        string rol = userDto.Rol?.ToLower() == "admin" ? "admin" : "user";
        await _userManager.AddToRoleAsync(utilizator, rol);

        return (utilizator, rol);
    }

    // ✅ Gestionare utilizatori
    public async Task<ApplicationUser> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<bool> UpdateUserAsync(ApplicationUser user)
    {
        var utilizator = await _userManager.FindByIdAsync(user.Id);
        if (utilizator == null)
            return false;

        utilizator.Email = user.Email;
        utilizator.PhoneNumber = user.PhoneNumber;
        utilizator.TwoFactorEnabled = user.TwoFactorEnabled;

        var result = await _userManager.UpdateAsync(utilizator);
        return result.Succeeded;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var utilizatori = await _userManager.Users.ToListAsync();
        var listaUtilizatori = new List<UserDto>();

        foreach (var user in utilizatori)
        {
            var roluri = await _userManager.GetRolesAsync(user);
            var rol = roluri.FirstOrDefault() ?? "user";

            listaUtilizatori.Add(new UserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Rol = rol
            });
        }

        return listaUtilizatori;
    }

    public async Task<bool> ExistaEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    public async Task<bool> ExistaTelefonAsync(string phoneNumber)
    {
        return await _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
    }

    // ✅ Gestionare roluri
    public async Task<bool> AtribuieRolAsync(string userId, string rol)
    {
        var utilizator = await _userManager.FindByIdAsync(userId);
        if (utilizator == null)
            return false;

        if (!await _roleManager.RoleExistsAsync(rol))
            await _roleManager.CreateAsync(new IdentityRole(rol));

        var rezultat = await _userManager.AddToRoleAsync(utilizator, rol);
        return rezultat.Succeeded;
    }

    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        var utilizator = await _userManager.FindByIdAsync(userId);
        return utilizator != null ? (await _userManager.GetRolesAsync(utilizator)).ToList() : new List<string>();
    }

    // ✅ Two-Factor Authentication (2FA)
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

    public async Task<bool> Activeaza2FAAsync(string userId)
    {
        var utilizator = await _userManager.FindByIdAsync(userId);
        if (utilizator == null)
            return false;

        utilizator.TwoFactorEnabled = true;
        return (await _userManager.UpdateAsync(utilizator)).Succeeded;
    }

    public async Task<bool> Dezactiveaza2FAAsync(string userId)
    {
        var utilizator = await _userManager.FindByIdAsync(userId);
        if (utilizator == null)
            return false;

        utilizator.TwoFactorEnabled = false;
        return (await _userManager.UpdateAsync(utilizator)).Succeeded;
    }
}
