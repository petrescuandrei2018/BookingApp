using System.Net.WebSockets;
using System.Threading.Tasks;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuWebSocket
    {
        void AddConnection(string connectionId, WebSocket socket);
        Task RemoveConnection(string connectionId);
        Task SendMessageToAll(string message);
        Task HandleWebSocketConnection(WebSocket webSocket);
    }
}
