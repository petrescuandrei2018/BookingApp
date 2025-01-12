namespace BookingApp.Models
{
    public class StripeSettings
    {
        public string PublishableKey { get; set; } // Cheia publică
        public string SecretKey { get; set; }      // Cheia secretă
        public string WebhookSecret { get; set; } // Secretul pentru webhook-uri
    }
}
