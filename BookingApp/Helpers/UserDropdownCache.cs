using BookingApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Helpers
{
    public static class UserDropdownCache
    {
        // Lista statică pentru utilizatori
        public static List<UserDropdownModel> Users { get; private set; } = new List<UserDropdownModel>();

        // Metodă pentru a popula lista cu utilizatori
        public static void PopulateUsers(IEnumerable<UserDropdownModel> users)
        {
            Users = users.ToList();
        }
    }
}
