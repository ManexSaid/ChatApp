using ChatApp.Shared;

namespace ChatApp.Client;


public class MessageHandler
{
    public event EventHandler<string>? TextMessageReceived;
    public event EventHandler<string>? SystemMessageReceived;

    public void ProcessMessage(ChatMessage message)
    {
        switch (message.Type)
        {
            case MessageType.Text:
                TextMessageReceived?.Invoke(this, $"{message.Sender}: {message.Content}");
                break;
            case MessageType.System:
                SystemMessageReceived?.Invoke(this, message.Content);
                break;
        }
    }
}