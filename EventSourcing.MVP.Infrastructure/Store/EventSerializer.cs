using EventSourcing.MVP.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace EventSourcing.MVP.Infrastructure.Store;

public static class EventSerializer
{
    private static readonly Dictionary<string, Type> _registeredEventTypes = new();

    public static void RegisterFromAssemblyContaining<T>() => RegisterEventTypes(typeof(T).Assembly.GetTypes().Where(x => typeof(IEvent).IsAssignableFrom(x)));

    public static void RegisterEventTypes(IEnumerable<Type> eventTypes)
    {
        foreach (var eventType in eventTypes)
        {
            if (!typeof(IEvent).IsAssignableFrom(eventType))
            {
                throw new ArgumentException(eventType.Name);
            }

            _registeredEventTypes.TryAdd(eventType.Name, eventType);
        }
    }

    public static IEvent Deserialize(Event evt)
    {
        if (!_registeredEventTypes.TryGetValue(evt.EventType, out var type))
        {
            throw new ArgumentOutOfRangeException(evt.EventType);
        }

        return (IEvent)JsonSerializer.Deserialize(evt.Payload, type);
    }

    public static string Serialize(IEvent evt)
    {
        return JsonSerializer.Serialize(evt as object);
    }
}
