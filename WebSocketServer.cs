using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RecklessBoon.MacroDeck.Discord
{
    public class WebSocketServer
    {
        private HttpListener _httpListener;
        private WebSocket _clientWebSocket;

        public event EventHandler<JObject> OnMessage;
        public event EventHandler<bool> OnConnectionStateChanged;

        public bool IsConnected => _clientWebSocket != null && _clientWebSocket.State == WebSocketState.Open;

        public void Start(int port = 8124)
        {
            Task.Run(async () =>
            {
                try
                {
                    _httpListener = new HttpListener();
                    _httpListener.Prefixes.Add($"http://127.0.0.1:{port}/");
                    _httpListener.Start();
                    PluginInstance.Logger.Info($"WebSocket Server started on port {port}");

                    while (true)
                    {
                        var context = await _httpListener.GetContextAsync();
                        if (context.Request.IsWebSocketRequest)
                        {
                            _ = ProcessRequest(context);
                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                            context.Response.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    PluginInstance.Logger.Error($"WebSocket Server Start Error: {ex.Message}");
                }
            });
        }

        private async Task ProcessRequest(HttpListenerContext context)
        {
            try
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);
                _clientWebSocket = webSocketContext.WebSocket;
                PluginInstance.Logger.Info("Client connected");
                OnConnectionStateChanged?.Invoke(this, true);

                await ReceiveLoop();
            }
            catch (Exception ex)
            {
                PluginInstance.Logger.Error($"WebSocket Accept error: {ex.Message}");
            }
            finally
            {
                OnConnectionStateChanged?.Invoke(this, false);
                _clientWebSocket = null;
            }
        }

        private async Task ReceiveLoop()
        {
            var buffer = new byte[1024 * 4];
            while (_clientWebSocket != null && _clientWebSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    }
                    else
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        try
                        {
                            var json = JObject.Parse(message);
                            OnMessage?.Invoke(this, json);
                        }
                        catch (Exception ex)
                        {
                            PluginInstance.Logger.Error($"Error parsing message: {ex.Message}");
                        }
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        public async Task Send(object data)
        {
            if (_clientWebSocket != null && _clientWebSocket.State == WebSocketState.Open)
            {
                var json = JsonConvert.SerializeObject(data);
                PluginInstance.Logger.Info($"Sending WS: {json}");
                var buffer = Encoding.UTF8.GetBytes(json);
                await _clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public void Stop()
        {
            if (_httpListener != null)
            {
                _httpListener.Stop();
                _httpListener.Close();
            }
            if (_clientWebSocket != null)
            {
                _clientWebSocket.Abort();
            }
        }
    }
}
