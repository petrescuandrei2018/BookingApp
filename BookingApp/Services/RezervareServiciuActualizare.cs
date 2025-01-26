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
            using (var scope = _furnizorServicii.CreateScope())
            {
                var _furnizorDateBd = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var _serviciuPlata = scope.ServiceProvider.GetRequiredService<IServiciuPlata>();
                var _serviciuEmail = scope.ServiceProvider.GetRequiredService<IServiciuEmail>();

                try
                {
                    var rezervari = await _furnizorDateBd.Rezervari
                        .Include(r => r.PretCamera)
                        .Where(r => r.PretCamera != null && r.PretCamera.PretNoapte > 0)
                        .ToListAsync();

                    foreach (var rezervare in rezervari)
                    {
                        var stareAnterioara = rezervare.Stare;
                        rezervare.Stare = DeterminaStareaRezervarii(rezervare.CheckIn, rezervare.CheckOut);

                        if (stareAnterioara != rezervare.Stare && rezervare.Stare == "Expirata")
                        {
                            // Refund automat
                            if (rezervare.SumaAchitata > 0)
                            {
                                await _serviciuPlata.ProceseazaRefundAsync(rezervare.ClientSecret, rezervare.SumaAchitata);
                                rezervare.SumaAchitata = 0;
                                rezervare.SumaRamasaDePlata = rezervare.SumaTotala;
                                rezervare.StarePlata = "Refundata";

                                // Trimite email utilizatorului
                                await _serviciuEmail.TrimiteEmailAsync(
                                    "email.utilizator@exemplu.com", // Înlocuiește cu emailul real al utilizatorului
                                    "Rezervare expirată și refund procesat",
                                    $"Rezervarea cu ID-ul {rezervare.RezervareId} a expirat. Refundul de {rezervare.SumaTotala} a fost procesat."
                                );
                            }

                            // Actualizează disponibilitatea camerei
                            var tipCamera = await _furnizorDateBd.TipCamere
                                .FirstOrDefaultAsync(tc => tc.TipCameraId == rezervare.PretCamera.TipCameraId);

                            if (tipCamera != null)
                            {
                                tipCamera.NrCamereDisponibile++;
                                tipCamera.NrCamereOcupate--;
                            }
                        }

                        var numarNopti = (rezervare.CheckOut - rezervare.CheckIn).Days;

                        if (numarNopti <= 0)
                        {
                            rezervare.Stare = "Expirata";
                            rezervare.SumaTotala = 0;
                            rezervare.SumaRamasaDePlata = 0;
                            rezervare.SumaAchitata = 0;
                            continue;
                        }

                        rezervare.SumaTotala = numarNopti * (decimal)rezervare.PretCamera.PretNoapte;
                        rezervare.SumaRamasaDePlata = rezervare.SumaTotala - rezervare.SumaAchitata;
                    }

                    await _furnizorDateBd.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Eroare în ActualizeazaRezervariAsync: {ex.Message}");
                }
            }
        }


        private string DeterminaStareaRezervarii(DateTime dataCheckIn, DateTime dataCheckOut)
        {
            var dataCurenta = DateTime.UtcNow;

            if (dataCheckOut < dataCurenta)
                return "Expirata";

            if (dataCheckIn <= dataCurenta && dataCheckOut >= dataCurenta)
                return "Activa";

            return "Viitoare";
        }

        protected override async Task ExecuteAsync(CancellationToken tokenAnulare)
        {
            while (!tokenAnulare.IsCancellationRequested)
            {
                try
                {
                    await ActualizeazaRezervariAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Eroare în ExecuteAsync: {ex.Message}");
                }

                // Rulare periodică
                await Task.Delay(TimeSpan.FromMinutes(5), tokenAnulare);
            }
        }
    }
}
