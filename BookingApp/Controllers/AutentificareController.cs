using BookingApp.Models;
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
}
