using Stripe;
using BookingApp.Data;
using BookingApp.Models;
using Microsoft.EntityFrameworkCore;
using BookingApp.Services.Abstractions;

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
                var rezervare = await _contextBd.Rezervari.FirstOrDefaultAsync(r => r.RezervareId == rezervareId);
                if (rezervare == null)
                {
                    throw new Exception($"Rezervarea cu ID-ul {rezervareId} nu există.");
                }

                if (rezervare.StarePlata == "Platita")
                {
                    throw new Exception($"Rezervarea cu ID-ul {rezervareId} a fost deja plătită.");
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
                rezervare.StarePlata = "In Progress";
                await _contextBd.SaveChangesAsync();

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
                var refundService = new RefundService(_stripeClient);
                var options = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                    Amount = suma.HasValue ? (long)(suma.Value * 100) : null
                };

                var refund = await refundService.CreateAsync(options);
                return refund.Status;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProceseazaRefundAsync] Eroare: {ex.Message}");
                throw new Exception($"Eroare la procesarea refund-ului: {ex.Message}");
            }
        }

        public async Task TrimiteEmailConfirmare(string email, string mesaj)
        {
            await _serviciuEmail.TrimiteEmailAsync(email, "Confirmare plată", mesaj);
        }
    }
}
