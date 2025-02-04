using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using BookingApp.Services.Abstractions;

namespace BookingApp.Services
{
    public class ServiciuWebSocket : IServiciuWebSocket
    {
        private readonly ConcurrentDictionary<string, WebSocket> _connections = new();

        public async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            var connectionId = Guid.NewGuid().ToString();
            _connections.TryAdd(connectionId, webSocket);

            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await RemoveConnection(connectionId);
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await SendMessageToAll($"[WebSocket] {connectionId}: {message}");
            }
        }

        public void AddConnection(string connectionId, WebSocket socket)
        {
            _connections.TryAdd(connectionId, socket);
        }

        public async Task RemoveConnection(string connectionId)
        {
            if (_connections.TryRemove(connectionId, out var socket))
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
            }
        }

        public async Task SendMessageToAll(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var tasks = _connections.Values
                .Where(socket => socket.State == WebSocketState.Open)
                .Select(socket => socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));

            await Task.WhenAll(tasks);
        }
    }
}
