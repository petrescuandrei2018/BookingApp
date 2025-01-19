namespace BookingApp.Helpers
{
    // Clasă statică pentru generarea cheilor utilizate în Redis.
    public static class RedisKeyHelper
    {
        // Generează o cheie pentru o rezervare în Redis.
        public static string GenereazaCheieRezervare(int rezervareId) => $"rezervare:{rezervareId}";

        // Generează o cheie pentru starea plății unei rezervări în Redis.
        public static string GenereazaCheiePlata(int rezervareId) => $"plata:{rezervareId}";

        // Generează o cheie pentru starea unui refund în Redis.
        public static string GenereazaCheieRefund(string paymentIntentId) => $"refund:{paymentIntentId}";

        // Generează o cheie pentru un utilizator în Redis.
        public static string GenereazaCheieUser(int userId) => $"user:{userId}";
    }
}
