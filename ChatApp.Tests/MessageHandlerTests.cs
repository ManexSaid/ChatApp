using ChatApp.Client;
using ChatApp.Shared;

namespace ChatApp.Tests;

public class MessageHandlerTests
{
    [Fact]
    public void ProcessMessage_TextMessage_RaisesTextMessageReceived()
    {
        // Arrange
        var handler = new MessageHandler();
        var message = new ChatMessage
        {
            Sender = "user1",
            Content = "Hello",
            Type = MessageType.Text
        };
        
        string? receivedMessage = null;
        handler.TextMessageReceived += (sender, msg) => receivedMessage = msg;

        // Act
        handler.ProcessMessage(message);

        // Assert
        Assert.Equal("user1: Hello", receivedMessage);
    }

    [Fact]
    public void ProcessMessage_SystemMessage_RaisesSystemMessageReceived()
    {
        // Arrange
        var handler = new MessageHandler();
        var message = new ChatMessage
        {
            Sender = "Server",
            Content = "User joined",
            Type = MessageType.System
        };
        
        string? receivedMessage = null;
        handler.SystemMessageReceived += (sender, msg) => receivedMessage = msg;

        // Act
        handler.ProcessMessage(message);

        // Assert
        Assert.Equal("User joined", receivedMessage);
    }
}