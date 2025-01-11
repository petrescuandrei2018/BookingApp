namespace BookingApp.Services.Abstractions
{
    public interface IServiciuEmail
    {
        Task TrimiteEmailAsync(string laEmail, string subiect, string corp);
    }

}
