using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using BookingApp.Services.Abstractions;

namespace BookingApp.Controllers
{
    [Route("api/websocket")]
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        private readonly IServiciuWebSocket _webSocketManager;

        public WebSocketController(IServiciuWebSocket webSocketManager)
        {
            _webSocketManager = webSocketManager;
        }

        [HttpGet("connect")]
        public async Task Connect()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var connectionId = Guid.NewGuid().ToString();
                _webSocketManager.AddConnection(connectionId, webSocket);

                await ReceiveMessages(webSocket, connectionId);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private async Task ReceiveMessages(WebSocket socket, string connectionId)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocketManager.RemoveConnection(connectionId);
                    break;
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await _webSocketManager.SendMessageToAll($"User {connectionId}: {message}");
                }
            }
        }
    }
}
