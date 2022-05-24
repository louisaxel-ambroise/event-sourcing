using EventSourcing.MVP.Infrastructure.Domain;
using EventSourcing.MVP.Infrastructure.Store;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.MVP.Infrastructure.Messaging;

public class Event
{
    public int Id { get; init; }
    public string AggregateType { get; init; }
    public string AggregateId { get; init; }
    public string EventType { get; init; }
    public string Payload { get; init; }
    public int Version { get; init; }
    public DateTime InsertedAt { get; init; }

    public static IEnumerable<Event> Create<T>(T aggregateRoot, IEnumerable<IEvent> events)
        where T : AggregateRoot
    {
        return events.Select((x, i) => new Event
        {
            AggregateId = aggregateRoot.Id,
            AggregateType = aggregateRoot.GetType().Name,
            Version = aggregateRoot.Version + i,
            EventType = x.GetType().Name,
            Payload = EventSerializer.Serialize(x)
        });
    }
}
