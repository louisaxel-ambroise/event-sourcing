using EventSourcing.MVP.Domain.Orders.Events;
using EventSourcing.MVP.Infrastructure.Consumers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Consumers;

public class OrderStatusEmailReaction : Reaction
{
    public int Count { get; private set; }

    public OrderStatusEmailReaction()
    {
        Register<OrderAllocatedToUser>(OrderAllocated);
        Register<OrderReleasedFromUser>(OrderReleased);
    }

    public override Task InitializeAsync(CancellationToken cancellationToken)
    {
        // TODO: use this method to make sure this is the only instance of this consumer.
        return base.InitializeAsync(cancellationToken);
    }

    protected override Task<int> GetLastProcessedEventIdAsync(CancellationToken cancellationToken) => Task.FromResult(-1);

    private static Task OrderAllocated(OrderAllocatedToUser _, CancellationToken cancellationToken)
    {
        Console.WriteLine("OrderInProgress Email sent successfully");

        return Task.CompletedTask;
    }

    private static Task OrderReleased(OrderReleasedFromUser _, CancellationToken cancellationToken)
    {
        Console.WriteLine("OrderStopped Email sent successfully");

        return Task.CompletedTask;
    }
}
