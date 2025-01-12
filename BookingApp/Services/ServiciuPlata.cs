using Stripe;
using BookingApp.Data;
using Microsoft.EntityFrameworkCore;
using BookingApp.Services.Abstractions;

namespace BookingApp.Services
{
    public class ServiciuPlata : IServiciuPlata
    {
        private readonly IStripeClient _clientStripe;
        private readonly AppDbContext _contextBd;
        private readonly IServiciuEmail _serviciuEmail;

        public ServiciuPlata(IStripeClient clientStripe, AppDbContext contextBd, IServiciuEmail serviciuEmail)
        {
            _clientStripe = clientStripe;
            _contextBd = contextBd;
            _serviciuEmail = serviciuEmail;
        }

        public async Task<string> ProceseazaPlataAsync(int rezervareId, decimal suma, string moneda, string descriere)
        {
            try
            {
                // Verificăm dacă rezervarea există
                var rezervare = await _contextBd.Rezervari.FirstOrDefaultAsync(r => r.RezervareId == rezervareId);
                if (rezervare == null)
                {
                    throw new Exception($"Rezervarea cu ID-ul {rezervareId} nu există.");
                }

                // Verificăm dacă rezervarea este deja plătită
                if (rezervare.StarePlata == "Platita")
                {
                    throw new Exception($"Rezervarea cu ID-ul {rezervareId} a fost deja plătită.");
                }

                // Configurăm serviciul de intenție de plată Stripe
                var serviciuIntentiePlata = new PaymentIntentService(_clientStripe);

                // Creăm intenția de plată
                PaymentIntent intentiePlata;
                try
                {
                    intentiePlata = await serviciuIntentiePlata.CreateAsync(new PaymentIntentCreateOptions
                    {
                        Amount = (long)(suma * 100), // Stripe folosește valoarea în cenți
                        Currency = moneda,
                        Description = descriere,
                        Metadata = new Dictionary<string, string>
                {
                    { "RezervareId", rezervareId.ToString() }
                }
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Eroare la crearea intenției de plată: {ex.Message}");
                }

                // Actualizăm starea plății în baza de date
                rezervare.StarePlata = "In Progress";
                await _contextBd.SaveChangesAsync();

                // Returnăm secretul clientului pentru procesarea plății
                return intentiePlata.ClientSecret;
            }
            catch (Exception ex)
            {
                // Logăm eroarea (dacă ai un serviciu de logare, înlocuiește Console.WriteLine)
                Console.WriteLine($"[ProceseazaPlataAsync] Eroare: {ex.Message}");
                throw;
            }
        }

        public async Task<string> ProceseazaRefundAsync(string paymentIntentId, decimal? suma)
        {
            try
            {
                var refundService = new RefundService(_clientStripe);

                // Configurăm opțiunile pentru refund
                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                    Amount = suma.HasValue ? (long?)(suma.Value * 100) : null // Stripe folosește cenți
                };

                // Creăm refund-ul
                var refund = await refundService.CreateAsync(refundOptions);

                Console.WriteLine($"[ProceseazaRefundAsync] Refund creat cu succes. Status: {refund.Status}");
                return refund.Status; // Returnăm statusul refund-ului
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProceseazaRefundAsync] Eroare la procesarea refund-ului: {ex.Message}");
                throw new Exception($"Eroare la procesarea refund-ului: {ex.Message}");
            }
        }


        public async Task TrimiteEmailConfirmare(string email, string mesaj)
        {
            await _serviciuEmail.TrimiteEmailAsync(email, "Confirmare plată", mesaj);
        }
    }

}
