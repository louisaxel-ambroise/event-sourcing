using EventSourcing.MVP.Infrastructure.Exceptions;
using EventSourcing.MVP.Infrastructure.Messaging;
using System.Collections.Generic;
using System.Reflection;

namespace EventSourcing.MVP.Infrastructure.Domain;

public abstract class AggregateRoot
{
    private readonly List<IEvent> _pendingEvents = new();

    public string Id { get; set; }
    public int Version { get; set; }

    public IEnumerable<IEvent> PendingEvents { get { return _pendingEvents.AsReadOnly(); } }

    public AggregateRoot Replay(IEvent evt) 
    {
        Handle(evt);
        Version++;

        return this;
    }

    protected void Apply(IEvent evt)
    {
        Handle(evt);
        _pendingEvents.Add(evt);
    }

    protected void Apply(IEnumerable<IEvent> events)
    {
        foreach(var evt in events)
        {
            Handle(evt);
        };
    }

    private void Handle(IEvent evt)
    {
        var method = GetType().GetMethod(nameof(Handle), BindingFlags.Instance | BindingFlags.NonPublic, new[] { evt.GetType() });

        if (method is null)
        {
            throw new UnprocessableEventException(evt.GetType().Name);
        }

        method.Invoke(this, new object[] { evt });
    }
}
