using EventSourcing.MVP.Infrastructure.Messaging;
using System;

namespace EventSourcing.MVP.Domain.Orders.Events;

public class OrderExpired : IEvent
{
    public DateTime ExpiredAt { get; set; }
    public string ExpiredBy { get; set; }
}

public class OrderLineCollected : IEvent
{
    public string Reference { get; set; }
    public string CollectedBy { get; set; }
    public DateTime CollectedAt { get; set; }
}
