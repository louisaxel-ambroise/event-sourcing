using EventSourcing.MVP.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Infrastructure.Consumers;

/// <summary>
/// Allows to register and retrieve event handlers for Projections and Reactions
/// </summary>
public class EventHandlerRegistry
{
    private readonly Dictionary<Type, Func<IEvent, CancellationToken, Task>> _handlers = new();

    public bool CanHandle(IEvent evt) => _handlers.ContainsKey(evt.GetType());
    
    public Task HandleAsync<T>(T evt, CancellationToken cancellationToken) 
        where T : IEvent
    {
        if (!CanHandle(evt))
        {
            throw new Exception("Event can't be handled.");
        }

        var handler = _handlers.GetValueOrDefault(evt.GetType()) as Func<T, CancellationToken, Task>;

        return handler(evt, cancellationToken);
    }

    public void Register<T>(Func<T, CancellationToken, Task> action)
        where T : IEvent
    {
        if (_handlers.ContainsKey(typeof(T)))
        {
            throw new Exception($"Handler already registered for type {typeof(T).Name}");
        }

        _handlers.Add(typeof(T), (IEvent evt, CancellationToken cancellationToken) => action((T) evt, cancellationToken));
    }
}