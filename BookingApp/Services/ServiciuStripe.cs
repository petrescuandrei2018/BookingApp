using Stripe;
using BookingApp.Services.Abstractions;
using Microsoft.Extensions.Configuration;

namespace BookingApp.Services
{
    public class ServiciuStripe : IServiciuStripe
    {
        private readonly IStripeClient _stripeClient;

        public ServiciuStripe(IConfiguration configuration)
        {
            var secretKey = configuration["Stripe:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Cheia secretă Stripe nu este configurată.");
            }

            _stripeClient = new StripeClient(secretKey);
        }

        public IStripeClient ObtineStripeClient()
        {
            return _stripeClient;
        }
    }
}
