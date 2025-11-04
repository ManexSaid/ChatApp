using ChatApp.Shared;

namespace ChatApp.Tests;

public class MessageSerializerTests
{
    [Fact]
    public void Serialize_ValidMessage_ReturnsJsonString()
    {
        // Arrange
        var message = new ChatMessage
        {
            Sender = "testuser",
            Content = "Hello world",
            Timestamp = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            Type = MessageType.Text
        };

        // Act
        var json = MessageSerializer.Serialize(message);

        // Assert
        Assert.NotNull(json);
        Assert.Contains("testuser", json);
        Assert.Contains("Hello world", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsChatMessage()
    {
        // Arrange
        var json = """{"sender":"testuser","content":"Hello world","timestamp":"2023-01-01T12:00:00Z","type":"Text"}""";

        // Act
        var message = MessageSerializer.Deserialize(json);

        // Assert
        Assert.Equal("testuser", message.Sender);
        Assert.Equal("Hello world", message.Content);
        Assert.Equal(MessageType.Text, message.Type);
    }

    [Fact]
    public void SerializeDeserialize_RoundTrip_ReturnsEquivalentMessage()
    {
        // Arrange
        var original = new ChatMessage
        {
            Sender = "user1",
            Content = "Test message",
            Timestamp = DateTime.UtcNow,
            Type = MessageType.System
        };

        // Act
        var json = MessageSerializer.Serialize(original);
        var deserialized = MessageSerializer.Deserialize(json);

        // Assert
        Assert.Equal(original.Sender, deserialized.Sender);
        Assert.Equal(original.Content, deserialized.Content);
        Assert.Equal(original.Type, deserialized.Type);
        Assert.Equal(original.Timestamp, deserialized.Timestamp);
    }
}