using EventSourcing.MVP.Infrastructure.Messaging;
using EventSourcing.MVP.Infrastructure.Store;
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
    protected override async Task ProcessEventsAsync(IEnumerable<Event> events, CancellationToken cancellationToken)
    {
        var tasks = events
                .Select(EventSerializer.Deserialize)
                .Where(CanHandle)
                .Select(HandleAsync);

        Task.WaitAll(tasks.ToArray(), cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    protected abstract bool CanHandle<T>(T evt) where T : IEvent;
    protected abstract Task HandleAsync<T>(T evt) where T : IEvent;
    protected abstract Task SaveChangesAsync(CancellationToken cancellationToken);
}
