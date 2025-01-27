using System.Net;
using System.Net.Mail;
using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace BookingApp.Services
{
    public class ServiciuPlata : IServiciuPlata
    {
        private readonly IStripeClient _stripeClient;
        private readonly AppDbContext _contextBd;
        private readonly IServiciuEmail _serviciuEmail;

        public ServiciuPlata(IStripeClient stripeClient, AppDbContext contextBd, IServiciuEmail serviciuEmail)
        {
            _stripeClient = stripeClient;
            _contextBd = contextBd;
            _serviciuEmail = serviciuEmail;
        }

        public async Task<string> ProceseazaPlataAsync(int rezervareId, decimal suma, string moneda, string descriere)
        {
            try
            {
                var rezervare = await _contextBd.Rezervari
                    .Include(r => r.User) // Include utilizatorul asociat
                    .FirstOrDefaultAsync(r => r.RezervareId == rezervareId);

                if (rezervare == null)
                {
                    throw new Exception($"Rezervarea cu ID-ul {rezervareId} nu există.");
                }

                if (rezervare.StarePlata == "Platita")
                {
                    throw new Exception($"Rezervarea cu ID-ul {rezervareId} a fost deja plătită integral.");
                }

                // Validare: suma nu trebuie să depășească suma rămasă de plată
                if (suma > rezervare.SumaRamasaDePlata)
                {
                    throw new Exception($"Suma trimisă ({suma} RON) depășește suma rămasă de plată ({rezervare.SumaRamasaDePlata} RON) pentru rezervarea cu ID-ul {rezervareId}.");
                }

                var paymentIntentService = new PaymentIntentService(_stripeClient);
                var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
                {
                    Amount = (long)(suma * 100),
                    Currency = moneda,
                    Description = descriere,
                    Metadata = new Dictionary<string, string>
            {
                { "RezervareId", rezervareId.ToString() }
            }
                });

                rezervare.ClientSecret = paymentIntent.ClientSecret;
                rezervare.SumaAchitata += suma;
                rezervare.SumaRamasaDePlata -= suma;
                rezervare.StarePlata = rezervare.SumaRamasaDePlata == 0 ? "Platita" : "In Progress";

                await _contextBd.SaveChangesAsync();

                // Trimite email de notificare
                if (rezervare.SumaRamasaDePlata == 0)
                {
                    await _serviciuEmail.TrimiteEmailAsync(
                        rezervare.User.Email,
                        "Plată integrală confirmată",
                        $"Rezervarea cu ID-ul {rezervare.RezervareId} a fost plătită integral."
                    );
                }
                else
                {
                    await _serviciuEmail.TrimiteEmailAsync(
                        rezervare.User.Email,
                        "Plată parțială procesată",
                        $"S-a achitat suma {suma} RON pentru rezervarea cu ID-ul {rezervare.RezervareId}. Mai aveți de plătit suma {rezervare.SumaRamasaDePlata} RON."
                    );
                }

                return paymentIntent.ClientSecret;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProceseazaPlataAsync] Eroare: {ex.Message}");
                throw;
            }
        }

        public async Task<string> ProceseazaRefundAsync(string paymentIntentId, decimal? suma)
        {
            try
            {
                var rezervare = await _contextBd.Rezervari
                    .Include(r => r.User) // Asigură-te că entitatea User este încărcată
                    .FirstOrDefaultAsync(r => r.ClientSecret == paymentIntentId);

                if (rezervare == null)
                {
                    throw new Exception("Rezervarea nu a fost găsită.");
                }

                if (rezervare.User == null)
                {
                    throw new Exception($"Rezervarea cu ID-ul {rezervare.RezervareId} nu are un utilizator asociat.");
                }

                var refundService = new RefundService(_stripeClient);
                var options = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                    Amount = suma.HasValue ? (long)(suma.Value * 100) : null
                };

                var refund = await refundService.CreateAsync(options);

                rezervare.SumaAchitata -= suma ?? rezervare.SumaAchitata;
                rezervare.SumaRamasaDePlata += suma ?? rezervare.SumaAchitata;
                rezervare.StarePlata = rezervare.SumaAchitata == 0 ? "Refundata" : "In Progress";

                await _contextBd.SaveChangesAsync();

                // Trimite email de notificare
                if (rezervare.SumaAchitata == 0)
                {
                    await _serviciuEmail.TrimiteEmailAsync(
                        rezervare.User.Email,
                        "Refund complet",
                        $"S-a efectuat un refund complet pentru rezervarea cu ID-ul {rezervare.RezervareId}."
                    );
                }
                else
                {
                    await _serviciuEmail.TrimiteEmailAsync(
                        rezervare.User.Email,
                        "Refund parțial procesat",
                        $"S-a efectuat un refund de {suma} RON pentru rezervarea cu ID-ul {rezervare.RezervareId}. Suma rămasă achitată este {rezervare.SumaAchitata} RON."
                    );
                }

                return refund.Status;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProceseazaRefundAsync] Eroare: {ex.Message}");
                throw new Exception($"Eroare la procesarea refund-ului: {ex.Message}");
            }
        }
    }
}
