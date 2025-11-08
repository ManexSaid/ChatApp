using System.Net;
using System.Net.Sockets;

namespace ChatApp.Server;

public sealed class ServerCore : IDisposable
{
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly List<ClientHandler> _clients;
    private readonly MessageDispatcher _dispatcher;
    private bool _isRunning;

    public ServerCore(int port = 13000)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _cancellationTokenSource = new CancellationTokenSource();
        _clients = new List<ClientHandler>();
        _dispatcher = new MessageDispatcher();
    }

    public async Task StartAsync()
    {
        _listener.Start();
        _isRunning = true;

        Console.WriteLine($"Server started on port {((IPEndPoint)_listener.LocalEndpoint).Port}");

        try
        {
            while (_isRunning)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync(_cancellationTokenSource.Token);
                _ = Task.Run(() => HandleClientAsync(tcpClient), _cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

    private async Task HandleClientAsync(TcpClient tcpClient)
    {
        var clientHandler = new ClientHandler(tcpClient, _dispatcher);

        lock (_clients)
        {
            _clients.Add(clientHandler);
        }

        _dispatcher.Subscribe(clientHandler);

        try
        {
            await clientHandler.StartAsync();
        }
        finally
        {
            _dispatcher.Unsubscribe(clientHandler);

            lock (_clients)
            {
                _clients.Remove(clientHandler);
            }

            Console.WriteLine($"Client disconnected. Total clients: {_clients.Count}");
        }
    }

    public void Stop()
    {
        if (!_isRunning) return;

        _isRunning = false;
        _cancellationTokenSource.Cancel();

        lock (_clients)
        {
            foreach (var client in _clients)
            {
                client.Dispose();
            }
            _clients.Clear();
        }

        _listener.Stop();
        _dispatcher.Dispose();
        Console.WriteLine("Server stopped gracefully.");
    }

    public void Dispose()
    {
        Stop();
        _cancellationTokenSource.Dispose();
    }
}