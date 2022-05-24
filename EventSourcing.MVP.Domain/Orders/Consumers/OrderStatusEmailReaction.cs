using EventSourcing.MVP.Domain.Orders.Events;
using EventSourcing.MVP.Infrastructure.Consumers;
using EventSourcing.MVP.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Consumers;

public class OrderStatusEmailReaction : Reaction
{
    readonly Dictionary<Type, Func<IEvent, Task>> _handlers = new();
    public int Count { get; private set; }

    public OrderStatusEmailReaction()
    {
        _handlers.Add(typeof(OrderAllocatedToUser), _ => OrderAllocated());
        _handlers.Add(typeof(OrderReleasedFromUser), _ => OrderReleased());
    }

    public override Task InitializeAsync(CancellationToken cancellationToken)
    {
        // TODO: use this method to make sure this is the only instance of this consumer.
        return base.InitializeAsync(cancellationToken);
    }

    protected override Task<int> GetLastProcessedEventIdAsync(CancellationToken cancellationToken) => Task.FromResult(-1);

    protected override bool CanHandle<T>(T evt)
    {
        return _handlers.ContainsKey(evt.GetType());
    }

    protected override Task HandleAsync<T>(T evt, CancellationToken cancellationToken)
    {
        if (!_handlers.TryGetValue(evt.GetType(), out var action))
        {
            throw new Exception();
        }

        return action(evt);
    }

    private static Task OrderAllocated()
    {
        Console.WriteLine("OrderInProgress Email sent successfully");

        return Task.CompletedTask;
    }

    private static Task OrderReleased()
    {
        Console.WriteLine("OrderStopped Email sent successfully");

        return Task.CompletedTask;
    }
}
