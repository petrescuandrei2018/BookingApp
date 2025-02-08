using BookingApp.Models;
using BookingApp.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Threading.Tasks;

[Route("api/autentificare")]
[ApiController]
public class AutentificareController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AutentificareController(IAuthService authService, UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] DateLogin model)
    {
        var utilizator = await _userManager.FindByEmailAsync(model.Email);
        if (utilizator == null || !await _authService.ValideazaUtilizatorAsync(model.Email, model.Parola))
            return Unauthorized("Email sau parolă incorectă.");

        if (utilizator.TwoFactorEnabled)
            return Ok(new { Mesaj = "2FA activ. Introduceți codul TOTP.", UserId = utilizator.Id });

        var token = await _authService.AutentificaUtilizatorAsync(model.Email, model.Parola);
        return Ok(new { Token = token });
    }


    [HttpPost("genereaza-2fa")]
    public async Task<IActionResult> Genereaza2FA([FromBody] string userId)
    {
        var key = await _authService.GenereazaCheie2FAAsync(userId);
        if (string.IsNullOrEmpty(key))
            return BadRequest("Eroare la generarea cheii 2FA.");

        string qrCodeUri = $"otpauth://totp/BookingApp?secret={key}&issuer=BookingApp";

        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new BitmapByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);
        var qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);

        return Ok(new { SecretKey = key, QRCodeBase64 = $"data:image/png;base64,{qrCodeBase64}" });
    }

    [HttpPost("verifica-2fa")]
    public async Task<IActionResult> Verifica2FA([FromBody] Verifica2FADto model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
            return Unauthorized(new { Mesaj = "Utilizator invalid." });

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, model.CodTOTP);
        if (!isValid)
            return Unauthorized(new { Mesaj = "Cod 2FA invalid." });

        var token = await _authService.AutentificaUtilizatorAsync(user.Email, model.Parola);
        return Ok(new { Token = token });
    }

    [HttpPost("activeaza-2fa")]
    public async Task<IActionResult> Activeaza2FA([FromBody] string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { Mesaj = "Utilizatorul nu a fost găsit." });

        user.TwoFactorEnabled = true;
        await _userManager.UpdateAsync(user);

        return Ok(new { Mesaj = "2FA activat cu succes." });
    }

    [HttpPost("dezactiveaza-2fa")]
    public async Task<IActionResult> Dezactiveaza2FA([FromBody] string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { Mesaj = "Utilizatorul nu a fost găsit." });

        user.TwoFactorEnabled = false;
        await _userManager.UpdateAsync(user);

        return Ok(new { Mesaj = "2FA dezactivat cu succes." });
    }


}
