using System.ComponentModel.DataAnnotations;

namespace BookingApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber {  get; set; }
        public int Varsta {  get; set; }
        public string Password { get; set; }
        public RolUtilizator Rol { get; set; } // Enum pentru roluri


        public User(int userId, string userName, string email, string phoneNumber, int varsta, string password)
        {
            UserId = userId;
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            Varsta = varsta;
            Password = password;
        }

        public User() { }
    }
}
