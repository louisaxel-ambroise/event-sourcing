using EventSourcing.MVP.Infrastructure.Messaging;
using System.Collections.Generic;
using System.Reflection;

namespace EventSourcing.MVP.Infrastructure.Domain;

public abstract class AggregateRoot
{
    private readonly Queue<IEvent> _pendingEvents = new();

    public string Id { get; set; }
    public int Version { get; set; }

    public void Handle(IEvent evt)
    {
        GetType().GetMethod("HandleEvent", BindingFlags.Instance | BindingFlags.NonPublic, new[] { evt.GetType() })
            .Invoke(this, new object[] { evt });
    }

    public AggregateRoot Apply<T>(T evt)
        where T : IEvent
    {
        Handle(evt);
        _pendingEvents.Enqueue(evt);

        return this;
    }

    public IEnumerable<IEvent> GetPendingEvents()
    {
        while (_pendingEvents.Count > 0)
        {
            yield return _pendingEvents.Dequeue();
        }
    }
}
