using EventSourcing.MVP.Infrastructure.Domain;
using EventSourcing.MVP.Infrastructure.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.MVP.Infrastructure.Store;

public static class AggregateRootLoader
{
    public static T LoadFromHistory<T>(string id, IEnumerable<IEvent> events)
      where T : AggregateRoot, new()
    {
        AggregateRoot seed = new T() { Id = id };

        return events.Aggregate(seed, (agg, evt) => agg.Replay(evt)) as T;
    }
}
