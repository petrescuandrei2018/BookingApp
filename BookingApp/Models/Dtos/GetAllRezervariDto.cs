namespace BookingApp.Models.Dtos
{
    public class GetAllRezervariDto
    {
        public int RezervareId { get; set; } // Generat automat

        public int UserId { get; set; } // Preluat din JWT

        public string? HotelName { get; set; } // Setat automat în backend
        public string? NumeCamera { get; set; } // Tipul camerei
        public DateTime CheckIn { get; set; } // Data Check-In
        public DateTime CheckOut { get; set; } // Data Check-Out

        public decimal? Pret { get; set; } // Calculat automat

        public string? Stare { get; set; } = "Viitoare"; // Implicit "Viitoare"

        public GetAllRezervariDto(int rezervareId, int userId, string? hotelName, string? numeCamera, DateTime checkIn, DateTime checkOut, decimal? pret, string? stare)
        {
            RezervareId = rezervareId;
            UserId = userId;
            HotelName = hotelName;
            NumeCamera = numeCamera;
            CheckIn = checkIn;
            CheckOut = checkOut;
            Pret = pret;
            Stare = stare;
        }
    }
}
