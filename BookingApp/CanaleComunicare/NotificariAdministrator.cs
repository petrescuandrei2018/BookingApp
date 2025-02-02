using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BookingApp.CanaleComunicare
{
    public class NotificariAdministrator : Hub
    {
        public async Task TrimiteNotificare(string mesaj)
        {
            await Clients.All.SendAsync("PrimesteNotificare", mesaj);
        }
    }
}
