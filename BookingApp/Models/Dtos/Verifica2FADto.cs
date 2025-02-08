namespace BookingApp.Models.Dtos
{
    public class Verifica2FADto
    {
        public string UserId { get; set; } = string.Empty;
        public string CodTOTP { get; set; } = string.Empty;
        public string Parola { get; set; } = string.Empty;
    }
}
