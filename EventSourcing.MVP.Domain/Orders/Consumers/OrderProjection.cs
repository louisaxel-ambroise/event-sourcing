using EventSourcing.MVP.Domain.Orders.Events;
using EventSourcing.MVP.Infrastructure.Consumers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Consumers;

public class OrderProjection : Projection
{
    public int Count { get; private set; }

    public OrderProjection()
    {
        Register<OrderCreated>(OrderCreated);
    }

    protected override Task<int> GetLastProcessedEventIdAsync(CancellationToken cancellationToken) => Task.FromResult(-1);

    protected override Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("[{0}] Number of orders: {1}", DateTime.UtcNow.ToLongTimeString(), Count);

        return Task.CompletedTask;
    }

    private Task OrderCreated(OrderCreated _, CancellationToken cancellationToken)
    {
        Count++;

        return Task.CompletedTask;
    }
}
