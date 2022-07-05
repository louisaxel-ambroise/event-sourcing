using EventSourcing.MVP.Infrastructure.Messaging;
using EventSourcing.MVP.Infrastructure.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Infrastructure.Consumers;

public abstract class Reaction : EventConsumer
{
    private readonly EventHandlerRegistry _handlers = new();
    
    protected override async Task ProcessEventsAsync(IEnumerable<Event> events, CancellationToken cancellationToken)
    {
        foreach (var evt in events.Select(EventSerializer.Deserialize).Where(CanHandle))
        {
            await HandleAsync(evt, cancellationToken);
        }
    }

    protected void Register<T>(Func<T, CancellationToken, Task> handler) where T : IEvent => _handlers.Register(handler);
    private bool CanHandle<T>(T evt) where T : IEvent => _handlers.CanHandle(evt);
    private Task HandleAsync<T>(T evt, CancellationToken cancellationToken) where T : IEvent => _handlers.HandleAsync(evt, cancellationToken);
}
