using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatApp.Server;

public enum MessageType
{
    Text,
    System,
    Join,
    Leave
}

public record ChatMessage
{
    public string Sender { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public MessageType Type { get; init; } = MessageType.Text;
}

public static class MessageSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = false
    };

    public static string Serialize(ChatMessage message)
    {
        return JsonSerializer.Serialize(message, Options);
    }

    public static ChatMessage Deserialize(string json)
    {
        return JsonSerializer.Deserialize<ChatMessage>(json, Options) 
            ?? throw new InvalidOperationException("Failed to deserialize message");
    }
}