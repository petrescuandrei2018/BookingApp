using BookingApp.Data;
using BookingApp.Services.Abstractions;

namespace BookingApp.Services
{
    public class RezervareServiciuActualizare : BackgroundService, IRezervareServiciuActualizare
    {
        private readonly AppDbContext _furnizorDateBd;
        public RezervareServiciuActualizare(AppDbContext furnizorDateBd)
        {
            _furnizorDateBd = furnizorDateBd;
        }
        public async Task RezervariUpdateAsync()
        {
            // Find expired bookings (CheckOut in the past and still Active)
            var rezervariExpirate = _furnizorDateBd.Rezervari.Where(r => r.CheckOut <DateTime.UtcNow && r.Status=="Active").ToList();
            
            foreach(var rezervare in rezervariExpirate)
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

        protected override async Task ExecuteAsync(CancellationToken tokenAnulare)
        {
            while (tokenAnulare.IsCancellationRequested == false)
            {
                await RezervariUpdateAsync();
                await Task.Delay(TimeSpan.FromSeconds(10), tokenAnulare); // Adjust delay as needed
            }
        }
    }
}
