using ChatApp.Server;

try
{
    var server = new ServerCore();
    Console.CancelKeyPress += (sender, e) =>
    {
        e.Cancel = true;
        server.Stop();
    };

    Console.WriteLine("Starting chat server...");
    await server.StartAsync();
    Console.WriteLine("Server stopped.");
}
catch (Exception ex)
{
    Console.WriteLine($"Server error: {ex.Message}");
}