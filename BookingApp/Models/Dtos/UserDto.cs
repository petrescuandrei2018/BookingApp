namespace BookingApp.Models.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class UserDto
    {
        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Range(1, 150)]
        public int Varsta { get; set; }

        [Required, MinLength(8, ErrorMessage = "Parola trebuie să aibă cel puțin 8 caractere")]
        public string Password { get; set; }
        public RolUtilizator Rol { get; set; } // Enum pentru roluri

    }
}
