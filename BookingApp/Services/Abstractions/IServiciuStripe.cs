using Stripe;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuStripe
    {
        IStripeClient ObtineStripeClient();
    }
}
