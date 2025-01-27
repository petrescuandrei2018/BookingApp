using BookingApp.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace BookingApp.Services
{
    public class FabricaServiciuEmail
    {
        private readonly IConfiguration _configuratie;

        public FabricaServiciuEmail(IConfiguration configuratie)
        {
            _configuratie = configuratie;
        }

        public IServiciuEmail CreeazaServiciuEmail()
        {
            var folosesteMock = _configuratie.GetValue<bool>("FolosesteEmailMock");
            return folosesteMock ? new ServiciuEmailMock() : new ServiciuEmailSmtp(_configuratie);
        }

        public bool EsteEmailValid(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        public async Task TrimiteEmailAsync(string laEmail, string subiect, string corp)
        {
            var serviciuEmail = CreeazaServiciuEmail();

            // Validare email înainte de trimitere
            if (!EsteEmailValid(laEmail))
            {
                throw new ArgumentException("Adresa de email nu este validă.");
            }

            await serviciuEmail.TrimiteEmailAsync(laEmail, subiect, corp);
        }
    }
}
