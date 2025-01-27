namespace BookingApp.Services.Abstractions
{
    public interface IServiciuPlata
    {
        Task<string> ProceseazaPlataAsync(int rezervareId, decimal suma, string moneda, string descriere);
        Task<string> ProceseazaRefundAsync(string paymentIntentId, decimal? suma);
    }
}
