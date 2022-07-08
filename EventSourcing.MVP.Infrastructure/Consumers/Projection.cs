using EventSourcing.MVP.Infrastructure.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Infrastructure.Consumers;

/// <summary>
/// Base class for a Projection - a process that transforms the event stream into a read model that is easier/faster to query.
/// The projection should be designed in such a way it can handle a batch of events at a time
/// </summary>
public abstract class Projection : EventConsumer
{
    protected override async Task ProcessEventsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken)
    {
        events.ToList().ForEach(async x => await HandleAsync(x, cancellationToken));

        await SaveChangesAsync(cancellationToken);
    }
    protected abstract Task SaveChangesAsync(CancellationToken cancellationToken);
}
