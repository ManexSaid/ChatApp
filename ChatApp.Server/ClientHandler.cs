using System.Net.Sockets;
using System.Text;
using ChatApp.Shared;

namespace ChatApp.Server;

public sealed class ClientHandler : IDisposable, IMessageObserver
{
    private readonly TcpClient _tcpClient;
    private readonly NetworkStream _stream;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly MessageDispatcher _dispatcher;
    private string? _username;

    public string Id { get; }
    public string Username => _username ?? "Unknown";

    public ClientHandler(TcpClient tcpClient, MessageDispatcher dispatcher)
    {
        _tcpClient = tcpClient;
        _stream = tcpClient.GetStream();
        _reader = new StreamReader(_stream, Encoding.UTF8);
        _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };
        _dispatcher = dispatcher;
        Id = Guid.NewGuid().ToString();
    }

    public async Task StartAsync()
    {
        try
        {

            var welcomeMessage = await _reader.ReadLineAsync();
            if (welcomeMessage != null)
            {
                var message = MessageSerializer.Deserialize(welcomeMessage);
                _username = message.Sender;
                Console.WriteLine($"Client connected: {_username} (ID: {Id})");


                await _dispatcher.BroadcastAsync(new ChatMessage
                {
                    Sender = "Server",
                    Content = $"{_username} joined the chat",
                    Timestamp = DateTime.UtcNow,
                    Type = MessageType.System
                });
            }

            while (_tcpClient.Connected)
            {
                var data = await _reader.ReadLineAsync();
                if (data == null) break;

                var message = MessageSerializer.Deserialize(data);


                if (message.Type == MessageType.Text)
                {
                    message = message with { Sender = _username ?? "Unknown", Timestamp = DateTime.UtcNow };
                }
                else
                {
                    message = message with { Timestamp = DateTime.UtcNow };
                }

                await _dispatcher.BroadcastAsync(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client {Username}: {ex.Message}");
        }
    }

    public async Task SendMessageAsync(ChatMessage message)
    {
        try
        {
            if (_tcpClient.Connected)
            {
                var serialized = MessageSerializer.Serialize(message);
                await _writer.WriteLineAsync(serialized);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message to {Username}: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _reader.Dispose();
        _writer.Dispose();
        _stream.Dispose();
        _tcpClient.Dispose();
    }
}