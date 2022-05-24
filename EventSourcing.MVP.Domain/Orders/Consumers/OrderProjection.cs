using EventSourcing.MVP.Domain.Orders.Events;
using EventSourcing.MVP.Infrastructure.Consumers;
using EventSourcing.MVP.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Consumers;

public class OrderProjection : Projection
{
    readonly Dictionary<Type, Func<IEvent, Task>> _handlers = new();
    public int Count { get; private set; }

    public OrderProjection()
    {
        _handlers.Add(typeof(OrderCreated), _ => OrderCreated());
    }

    protected override Task<int> GetLastProcessedEventIdAsync(CancellationToken cancellationToken) => Task.FromResult(-1);

    protected override bool CanHandle<T>(T evt)
    {
        return _handlers.ContainsKey(evt.GetType());
    }

    protected override Task HandleAsync<T>(T evt)
    {
        if (!_handlers.TryGetValue(evt.GetType(), out var action))
        {
            throw new Exception();
        }

        return action(evt);
    }

    protected override Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("[{0}] Number of orders: {1}", DateTime.UtcNow.ToLongTimeString(), Count);

        return Task.CompletedTask;
    }

    private Task OrderCreated()
    {
        Count++;

        return Task.CompletedTask;
    }
}
