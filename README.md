# Modular Chat Application

A TCP-based chat system demonstrating advanced C# concepts with a focus on code quality, design patterns, and mindful memory usage.

## Features

- Real-time messaging between multiple clients
- System notifications for user join/leave
- Modular architecture with clear separation of concerns
- Proper resource disposal and memory management
- Unit tests for core components

## Advanced C# Concepts Demonstrated

1. **Asynchronous Programming**: Full async/await pattern for network operations
2. **Generics and Collections**: Type-safe collections and generic methods
3. **Delegates and Events**: Event-based message handling
4. **Records**: Immutable data types for messages
5. **Pattern Matching**: Switch expressions for message type handling

## Design Patterns

- **Mediator Pattern**: `MessageDispatcher` decouples client communication
- **Observer Pattern**: Clients observe the message dispatcher
- **IDisposable Pattern**: Proper resource cleanup

## Memory Management

- Explicit disposal of network streams and readers/writers
- Cancellation tokens for graceful shutdown
- `ReaderWriterLockSlim` for efficient concurrent access
- Avoidance of memory leaks through proper event unsubscription

## Build and Run

### Prerequisites
- .NET 9.0 SDK

### Steps

1. **Build the solution**:
   ```bash
   dotnet build