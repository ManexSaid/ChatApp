using System.Net.Sockets;
using System.Text;
using ChatApp.Shared;

namespace ChatApp.Client;

public sealed class ChatClient : IDisposable
{
    private TcpClient? _tcpClient;
    private NetworkStream? _stream;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public event EventHandler<ChatMessage>? MessageReceived;
    public bool IsConnected => _tcpClient?.Connected == true;

    public ChatClient()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task ConnectAsync(string host, int port, string username)
    {
        _tcpClient = new TcpClient();
        await _tcpClient.ConnectAsync(host, port);

        _stream = _tcpClient.GetStream();
        _reader = new StreamReader(_stream, Encoding.UTF8);
        _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };


        var joinMessage = new ChatMessage
        {
            Sender = username,
            Content = $"{username} has joined",
            Type = MessageType.Join,
            Timestamp = DateTime.UtcNow
        };

        await _writer.WriteLineAsync(MessageSerializer.Serialize(joinMessage));


        _ = Task.Run(ListenForMessagesAsync, _cancellationTokenSource.Token);
    }

    private async Task ListenForMessagesAsync()
    {
        try
        {
            while (IsConnected && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var data = await _reader!.ReadLineAsync();
                if (data == null) break;

                var message = MessageSerializer.Deserialize(data);
                MessageReceived?.Invoke(this, message);
            }
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            if (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
        }
    }

    public async Task SendMessageAsync(string content)
    {
        if (!IsConnected || _writer == null) return;

        var message = new ChatMessage
        {
            Sender = "User",
            Content = content,
            Timestamp = DateTime.UtcNow,
            Type = MessageType.Text
        };

        await _writer.WriteLineAsync(MessageSerializer.Serialize(message));
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _reader?.Dispose();
        _writer?.Dispose();
        _stream?.Dispose();
        _tcpClient?.Dispose();
        _cancellationTokenSource.Dispose();
    }
}