using EventSourcing.MVP.Infrastructure.Messaging;
using System;

namespace EventSourcing.MVP.Domain.Orders.Events;

public class OrderCreated : IEvent
{
    public string Site { get; set; }
    public string ExpectedCarrier { get; set; }
    public DateTimeOffset PlacedOn { get; set; }
}
