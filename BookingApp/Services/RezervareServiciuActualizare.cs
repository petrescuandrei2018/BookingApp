using BookingApp.Data;
using BookingApp.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class RezervareServiciuActualizare : BackgroundService, IRezervareServiciuActualizare
    {
        private readonly IServiceProvider _furnizorServicii;

        public RezervareServiciuActualizare(IServiceProvider furnizorServicii)
        {
            _furnizorServicii = furnizorServicii;
        }

        public async Task RezervariUpdateAsync()
        {
            // Create a new scope to resolve the scoped AppDbContext
            using (var scope = _furnizorServicii.CreateScope())
            {
                var _furnizorDateBd = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Find expired bookings (CheckOut in the past and still Active)
                var rezervariExpirate = _furnizorDateBd.Rezervari
                    .Include(r => r.PretCamera)
                    .Where(r => r.CheckOut < DateTime.UtcNow && r.Status == "Active")
                    .ToList();

                foreach (var rezervare in rezervariExpirate)
                {
                    rezervare.Status = "Inactive";
                    var tipCamera = _furnizorDateBd.TipCamere
                        .FirstOrDefault(tc => tc.TipCameraId == rezervare.PretCamera.TipCameraId);
                    if (tipCamera != null)
                    {
                        tipCamera.NrCamereOcupate--;
                        tipCamera.NrCamereDisponibile++;
                    }
                }

                await _furnizorDateBd.SaveChangesAsync();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken tokenAnulare)
        {
            while (!tokenAnulare.IsCancellationRequested)
            {
                await RezervariUpdateAsync();
                await Task.Delay(TimeSpan.FromSeconds(10), tokenAnulare); // Adjust delay as needed
            }
        }
    }
}