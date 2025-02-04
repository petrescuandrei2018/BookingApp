using System.Security.Claims;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuUtilizator
    {
        int ObtineUtilizatorIdDinClaims(ClaimsPrincipal utilizator);
    }
}
