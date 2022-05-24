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
        return events.Aggregate(new T() { Id = id }, Replay);
    }

    private static TAgg Replay<TAgg, TEvt>(TAgg aggregate, TEvt evt)
        where TAgg : AggregateRoot
        where TEvt : IEvent
    {
        aggregate.Handle(evt);
        aggregate.Version++;

        return aggregate;
    }
}
