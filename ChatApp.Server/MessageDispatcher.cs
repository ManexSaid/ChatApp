using ChatApp.Shared;

namespace ChatApp.Server;


public sealed class MessageDispatcher : IDisposable
{
    private readonly List<IMessageObserver> _observers;
    private readonly ReaderWriterLockSlim _lock;

    public MessageDispatcher()
    {
        _observers = new List<IMessageObserver>();
        _lock = new ReaderWriterLockSlim();
    }

    public void Subscribe(IMessageObserver observer)
    {
        _lock.EnterWriteLock();
        try
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Unsubscribe(IMessageObserver observer)
    {
        _lock.EnterWriteLock();
        try
        {
            _observers.Remove(observer);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public async Task BroadcastAsync(ChatMessage message)
    {
        List<IMessageObserver> observersCopy;

        _lock.EnterReadLock();
        try
        {
            observersCopy = new List<IMessageObserver>(_observers);
        }
        finally
        {
            _lock.ExitReadLock();
        }


        var tasks = observersCopy.Select(observer =>
            observer.SendMessageAsync(message));

        await Task.WhenAll(tasks);
    }

    public void Dispose()
    {
        _lock.Dispose();
    }
}

public interface IMessageObserver
{
    Task SendMessageAsync(ChatMessage message);
}