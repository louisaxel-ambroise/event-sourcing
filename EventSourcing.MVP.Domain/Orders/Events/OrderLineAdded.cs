using EventSourcing.MVP.Infrastructure.Messaging;
using System;

namespace EventSourcing.MVP.Domain.Orders.Events;

public class OrderLineAdded : IEvent
{
    public OrderLine.OrderLineType Type { get; set; }
    public OrderLine.OrderLineStatus Status { get; set; }
    public string Reference { get; set; }
    public string ExternalId { get; set; }
    public string Sku { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset? DeliveryDeadline { get; set; }
}
