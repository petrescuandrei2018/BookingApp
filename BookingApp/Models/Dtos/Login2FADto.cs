public class Login2FADto
{
    public string Email { get; set; }
    public string Parola { get; set; }
    public string? CodTOTP { get; set; }  // Cod 2FA (opțional inițial)
}
