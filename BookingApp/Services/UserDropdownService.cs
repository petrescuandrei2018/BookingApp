using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingApp.Data;
using BookingApp.Models;

namespace BookingApp.Services
{
    public class UserDropdownService : IUserDropdownService
    {
        private readonly AppDbContext _context;

        public UserDropdownService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDropdownModel>> GetUsersForDropdownAsync()
        {
            var users = await Task.Run(() =>
                _context.Users.Select(user => new UserDropdownModel
                {
                    UserId = user.UserId.ToString(),
                    DisplayName = $"{user.UserName} ({user.Rol})"
                }).ToList()
            );

            return users;
        }
    }
}
