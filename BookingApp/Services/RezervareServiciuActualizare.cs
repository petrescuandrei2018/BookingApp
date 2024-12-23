using BookingApp.Data;
using BookingApp.Models;
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

        public async Task ActualizeazaRezervariAsync()
        {
            
            using (var contextServicii = _furnizorServicii.CreateScope())
            {
                var _furnizorDateBd = contextServicii.ServiceProvider.GetRequiredService<AppDbContext>();

                // Update statuses for existing bookings
                var rezervari = await _furnizorDateBd.Rezervari
                    .Include(r => r.PretCamera)
                    .ToListAsync();

                foreach (var rezervare in rezervari)
                {
                    var stareAnterioara = rezervare.Stare;
                    rezervare.Stare = DeterminaStareaRezervarii(rezervare.CheckIn, rezervare.CheckOut);

                    // Adjust room counts only when transitioning to Expirata
                    if ((stareAnterioara == StareRezervare.Activa || stareAnterioara == StareRezervare.Viitoare) &&
                        rezervare.Stare == StareRezervare.Expirata)
                    {
                        var tipCamera = _furnizorDateBd.TipCamere
                            .FirstOrDefault(tc => tc.TipCameraId == rezervare.PretCamera.TipCameraId);

                        if (tipCamera != null)
                        {
                            tipCamera.NrCamereDisponibile++;
                            tipCamera.NrCamereOcupate--;
                        }
                    }
                }

                await _furnizorDateBd.SaveChangesAsync();
            }
        }

        private StareRezervare DeterminaStareaRezervarii(DateTime dataCheckIn, DateTime dataCheckOut)
        {
            var dataCurenta = DateTime.UtcNow;

            if (dataCheckOut < dataCurenta)
                return StareRezervare.Expirata;

            if (dataCheckIn <= dataCurenta && dataCheckOut >= dataCurenta)
                return StareRezervare.Activa;

            return StareRezervare.Viitoare;
        }

        protected override async Task ExecuteAsync(CancellationToken tokenAnulare)
        {
            while (!tokenAnulare.IsCancellationRequested)
            {
                await ActualizeazaRezervariAsync();
                await Task.Delay(TimeSpan.FromSeconds(10), tokenAnulare); // Adjust frequency as needed
            }
        }
    }
}