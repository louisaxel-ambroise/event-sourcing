using EventSourcing.MVP.Infrastructure.Messaging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Infrastructure.Store;

public interface IEventStore
{
    Task<IEnumerable<Event>> LoadEventsAsync(string aggregateType, string aggregateId, CancellationToken cancellationToken);
    Task StoreAsync(IEnumerable<Event> events, CancellationToken cancellationToken);
    Task<IEnumerable<Event>> ListEventsAsync(int startFromExclusive, int maxEventCount, CancellationToken cancellationToken);
}
