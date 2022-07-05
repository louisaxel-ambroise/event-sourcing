using EventSourcing.MVP.Infrastructure.Exceptions;
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
        var method = GetType().GetMethod(nameof(Handle), BindingFlags.Instance | BindingFlags.NonPublic, new[] { evt.GetType() });

        if (method is null)
        {
            throw new UnprocessableEventException(evt.GetType().Name);
        }
        
        method.Invoke(this, new object[] { evt });
    }

    public IEnumerable<IEvent> GetPendingEvents()
    {
        while (_pendingEvents.Count > 0)
        {
            yield return _pendingEvents.Dequeue();
        }
    }

    protected AggregateRoot Apply(IEvent evt) 
    {
        Handle(evt);
        _pendingEvents.Enqueue(evt);

        return this;
    }

    protected AggregateRoot Apply(IEnumerable<IEvent> events)
    {
        foreach(var evt in events)
        {
            Apply(evt);
        };

        return this;
    }
}
