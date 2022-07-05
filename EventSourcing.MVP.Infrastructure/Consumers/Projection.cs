using EventSourcing.MVP.Infrastructure.Messaging;
using EventSourcing.MVP.Infrastructure.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Infrastructure.Consumers;

/// <summary>
/// Base class for a Projection - a process that transforms the event stream into a 
/// read model that is easier/faster to query.
/// The projection should be designed in such a way it can handle a batch of events at a time
/// </summary>
public abstract class Projection : EventConsumer
{
    private readonly EventHandlerRegistry _handlers = new();

    protected override async Task ProcessEventsAsync(IEnumerable<Event> events, CancellationToken cancellationToken)
    {
        var tasks = events
                .Select(EventSerializer.Deserialize)
                .Where(CanHandle)
                .Select(x => HandleAsync(x, cancellationToken));

        Task.WaitAll(tasks.ToArray(), cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    protected void Register<T>(Func<T, CancellationToken, Task> handler) where T : IEvent => _handlers.Register(handler);
    private bool CanHandle<T>(T evt) where T : IEvent => _handlers.CanHandle(evt);
    private Task HandleAsync<T>(T evt, CancellationToken cancellationToken) where T : IEvent => _handlers.HandleAsync(evt, cancellationToken);
    protected abstract Task SaveChangesAsync(CancellationToken cancellationToken);
}
