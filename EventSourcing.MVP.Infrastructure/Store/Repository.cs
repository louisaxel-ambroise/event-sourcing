using EventSourcing.MVP.Infrastructure.Domain;
using EventSourcing.MVP.Infrastructure.Exceptions;
using EventSourcing.MVP.Infrastructure.Messaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Infrastructure.Store;

public class Repository<T>
        where T : AggregateRoot, new()
{
    private readonly IEventStore _eventStore;

    public Repository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<T> LoadAsync(string id, CancellationToken cancellationToken)
    {
        var events = await _eventStore.LoadEventsAsync(typeof(T).Name, id, cancellationToken);

        if (!events.Any())
        {
            throw new MissingAggregateException(typeof(T).Name, id);
        }

        return AggregateRootLoader.LoadFromHistory<T>(id, events.Select(EventSerializer.Deserialize));
    }

    public Task SaveAsync(T aggregateRoot, CancellationToken cancellationToken)
    {
        var pendingEvents = Event.Create(aggregateRoot, aggregateRoot.PendingEvents);

        if (!pendingEvents.Any())
        {
            return Task.CompletedTask;
        }
        
        return _eventStore.StoreAsync(pendingEvents, cancellationToken);
    }
}
