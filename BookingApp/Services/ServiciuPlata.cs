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
            var rezervare = await _contextBd.Rezervari.FirstOrDefaultAsync(r => r.RezervareId == rezervareId);
            if (rezervare == null)
                throw new Exception("Rezervarea nu există.");

            if (rezervare.StarePlata == "Platita")
                throw new Exception("Rezervarea a fost deja plătită.");

            // Configurăm plata folosind Stripe
            var serviciuIntentiePlata = new PaymentIntentService(_clientStripe);
            var intentiePlata = await serviciuIntentiePlata.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = (long)(suma * 100),
                Currency = moneda,
                Description = descriere,
                Metadata = new Dictionary<string, string>
            {
                { "RezervareId", rezervareId.ToString() }
            }
            });

            rezervare.StarePlata = "In Progress";
            await _contextBd.SaveChangesAsync();

            return intentiePlata.ClientSecret;
        }

        public async Task TrimiteEmailConfirmare(string email, string mesaj)
        {
            await _serviciuEmail.TrimiteEmailAsync(email, "Confirmare plată", mesaj);
        }
    }

}
