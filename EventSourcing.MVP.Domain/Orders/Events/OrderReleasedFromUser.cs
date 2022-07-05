using EventSourcing.MVP.Infrastructure.Messaging;
using System;

namespace EventSourcing.MVP.Domain.Orders.Events;

public class OrderReleasedFromUser : IEvent
{
    public DateTime ReleasedOn { get; set; }
    public string ReleasedFrom { get; set; }
}
