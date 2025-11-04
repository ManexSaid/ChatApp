using ChatApp.Client;
using ChatApp.Shared;

Console.WriteLine("Chat Client - Enter your username:");
var username = Console.ReadLine();

if (string.IsNullOrWhiteSpace(username))
{
    Console.WriteLine("Invalid username.");
    return;
}

using var client = new ChatClient();
client.MessageReceived += (sender, message) =>
{
    var timestamp = message.Timestamp.ToLocalTime().ToString("HH:mm");
    var prefix = message.Type == MessageType.System ? "[System] " : "";
    Console.WriteLine($"[{timestamp}] {prefix}{message.Sender}: {message.Content}");
};

try
{
    await client.ConnectAsync("localhost", 13000, username);
    Console.WriteLine($"Connected as {username}. Type your messages (type '/exit' to quit):");

    while (true)
    {
        var input = Console.ReadLine();
        if (string.IsNullOrEmpty(input)) continue;
        
        if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase))
            break;

        await client.SendMessageAsync(input);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}