namespace BookingApp.Models.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class UserDto
    {
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Formatul email-ului nu este valid.")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Varsta { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; } = "user"; // Schimbat în string
    }

}
