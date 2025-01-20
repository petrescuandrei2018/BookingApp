using System.ComponentModel.DataAnnotations;

namespace BookingApp.Models.Dtos
{
    public class LogInDto
    {
        [Required(ErrorMessage = "Email-ul este obligatoriu.")]
        [EmailAddress(ErrorMessage = "Email-ul nu este valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola este obligatorie.")]
        [MinLength(5, ErrorMessage = "Parola trebuie să aibă cel puțin 5 caractere.")]
        public string Password { get; set; }
    }
}
