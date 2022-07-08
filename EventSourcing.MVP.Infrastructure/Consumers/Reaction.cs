using EventSourcing.MVP.Infrastructure.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Infrastructure.Consumers;

public abstract class Reaction : EventConsumer
{
    protected override async Task ProcessEventsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken)
    {
        foreach (var evt in events.Where(CanHandle))
        {
            await HandleAsync(evt, cancellationToken);
        }
    }
}
