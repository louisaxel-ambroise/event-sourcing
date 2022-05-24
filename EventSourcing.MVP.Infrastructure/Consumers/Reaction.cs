using EventSourcing.MVP.Infrastructure.Messaging;
using EventSourcing.MVP.Infrastructure.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Infrastructure.Consumers;

public abstract class Reaction : EventConsumer
{
    protected override async Task ProcessEventsAsync(IEnumerable<Event> events, CancellationToken cancellationToken)
    {
        foreach (var evt in events.Select(EventSerializer.Deserialize).Where(CanHandle))
        {
            await HandleAsync(evt, cancellationToken);
        }
    }

    protected abstract bool CanHandle<T>(T evt) where T : IEvent;
    protected abstract Task HandleAsync<T>(T evt, CancellationToken cancellationToken) where T : IEvent;
}
