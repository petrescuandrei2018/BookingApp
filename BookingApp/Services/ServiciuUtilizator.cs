using System;
using System.Security.Claims;
using BookingApp.Services.Abstractions;

namespace BookingApp.Services
{
    public class ServiciuUtilizator : IServiciuUtilizator
    {
        public int ObtineUtilizatorIdDinClaims(ClaimsPrincipal utilizator)
        {
            if (utilizator == null)
            {
                throw new ArgumentNullException(nameof(utilizator), "Utilizatorul nu este autentificat.");
            }

            var claim = utilizator.Claims.FirstOrDefault(c => c.Type == "UtilizatorId");
            if (claim == null)
            {
                throw new Exception("Nu s-a găsit identificatorul utilizatorului în claims.");
            }

            return int.Parse(claim.Value);
        }
    }
}
