using EventSourcing.MVP.Infrastructure.Messaging;
using System;

namespace EventSourcing.MVP.Domain.Orders.Events;

public class OrderAllocatedToUser : IEvent
{
    public DateTime AllocatedOn { get; set; }
    public string Username { get; set; }
}
