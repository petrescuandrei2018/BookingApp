using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    // Serviciu pentru actualizarea automată a rezervărilor
    public class RezervareServiciuActualizare : BackgroundService, IRezervareServiciuActualizare
    {
        // Furnizorul de servicii utilizat pentru a crea scope-uri
        private readonly IServiceProvider _furnizorServicii;

        // Constructor pentru injectarea dependențelor
        public RezervareServiciuActualizare(IServiceProvider furnizorServicii)
        {
            _furnizorServicii = furnizorServicii;
        }

        // Metodă pentru actualizarea rezervărilor existente
        public async Task ActualizeazaRezervariAsync()
        {
            using (var contextServicii = _furnizorServicii.CreateScope())
            {
                var _furnizorDateBd = contextServicii.ServiceProvider.GetRequiredService<AppDbContext>();

                try
                {
                    // Obținem toate rezervările, incluzând camerele, dar gestionăm cazurile de `null`
                    var rezervari = await _furnizorDateBd.Rezervari
                        .Include(r => r.PretCamera)
                        .Where(r => r.PretCamera != null && r.PretCamera.PretNoapte > 0) // Asigură-te că Prețul camerei este valid
                        .ToListAsync();

                    foreach (var rezervare in rezervari)
                    {
                        // Salvăm starea anterioară a rezervării
                        var stareAnterioara = rezervare.Stare;

                        // Determinăm noua stare a rezervării pe baza datelor CheckIn și CheckOut
                        rezervare.Stare = DeterminaStareaRezervarii(rezervare.CheckIn, rezervare.CheckOut);

                        // Ajustăm numărul de camere doar când rezervarea trece în starea Expirata
                        if ((stareAnterioara == "Activa" || stareAnterioara == "Viitoare") && rezervare.Stare == "Expirata")
                        {
                            var tipCamera = _furnizorDateBd.TipCamere
                                .FirstOrDefault(tc => tc.TipCameraId == rezervare.PretCamera.TipCameraId);

                            if (tipCamera != null)
                            {
                                // Incrementăm camerele disponibile și decrementăm camerele ocupate
                                tipCamera.NrCamereDisponibile++;
                                tipCamera.NrCamereOcupate--;
                            }
                        }

                        // Calculăm numărul de nopți pentru rezervare
                        var numarNopti = (rezervare.CheckOut - rezervare.CheckIn).Days;

                        // Verificăm dacă numărul de nopți este valid
                        if (numarNopti <= 0)
                        {
                            rezervare.Stare = "Expirata";
                            rezervare.SumaTotala = 0;
                            rezervare.SumaRamasaDePlata = 0;
                            rezervare.SumaAchitata = 0;
                            continue;
                        }

                        // Verificăm prețul pe noapte
                        if (rezervare.PretCamera?.PretNoapte <= 0)
                        {
                            rezervare.SumaTotala = 0;
                            rezervare.SumaRamasaDePlata = 0;
                            rezervare.SumaAchitata = 0;
                            continue;
                        }

                        // Calculăm sumele
                        rezervare.SumaTotala = numarNopti * (decimal)rezervare.PretCamera.PretNoapte;
                        rezervare.SumaRamasaDePlata = rezervare.SumaTotala - rezervare.SumaAchitata;
                    }

                    // Salvăm modificările în baza de date
                    await _furnizorDateBd.SaveChangesAsync();
                }
                catch (SqlNullValueException ex)
                {
                    // Loghează eroarea sau gestionează mai bine excepțiile în funcție de necesități
                    Console.WriteLine($"A apărut o eroare: {ex.Message}");
                }
            }
        }

        // Metodă pentru determinarea stării rezervării
        private string DeterminaStareaRezervarii(DateTime dataCheckIn, DateTime dataCheckOut)
        {
            var dataCurenta = DateTime.UtcNow;

            if (dataCheckOut < dataCurenta)
                return "Expirata";

            if (dataCheckIn <= dataCurenta && dataCheckOut >= dataCurenta)
                return "Activa";

            return "Viitoare";
        }

        // Metodă principală care rulează în fundal pentru actualizarea periodică a rezervărilor
        protected override async Task ExecuteAsync(CancellationToken tokenAnulare)
        {
            while (!tokenAnulare.IsCancellationRequested)
            {
                // Actualizăm rezervările
                await ActualizeazaRezervariAsync();

                // Așteptăm înainte de următoarea rulare
                await Task.Delay(TimeSpan.FromSeconds(10), tokenAnulare); // Ajustați frecvența după necesitate
            }
        }
    }
}
