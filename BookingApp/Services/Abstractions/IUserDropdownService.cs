// Interfata IUserDropdownService
using BookingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public interface IUserDropdownService
    {
        Task<List<UserDropdownModel>> GetUsersForDropdownAsync();
    }
}
