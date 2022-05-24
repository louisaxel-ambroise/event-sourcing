using EventSourcing.MVP.Infrastructure.Domain;
using EventSourcing.MVP.Infrastructure.Exceptions;
using EventSourcing.MVP.Infrastructure.Messaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Infrastructure.Store;

public class Repository
{
    private readonly IEventStore _eventStore;

    public Repository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<T> LoadAsync<T>(string id, CancellationToken cancellationToken)
        where T : AggregateRoot, new()
    {
        var events = await _eventStore.LoadEventsAsync(typeof(T).Name, id, cancellationToken);

        if (!events.Any())
        {
            throw new MissingAggregateException(typeof(T).Name, id);
        }

        return AggregateRootLoader.LoadFromHistory<T>(id, events.Select(EventSerializer.Deserialize));
    }

    public Task SaveAsync<T>(T aggregateRoot, CancellationToken cancellationToken)
        where T : AggregateRoot
    {
        var pendingEvents = Event.Create(aggregateRoot, aggregateRoot.GetPendingEvents());

        return _eventStore.StoreAsync(pendingEvents, cancellationToken);
    }
}
