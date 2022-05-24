using EventSourcing.MVP.Domain.Orders.Events;
using EventSourcing.MVP.Infrastructure.Domain;
using EventSourcing.MVP.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.MVP.Domain.Orders;

public class Order : AggregateRoot
{
    public string Site { get; set; }
    public string ExpectedCarrier { get; set; }
    public DateTimeOffset PlacedOn { get; set; }
    public string AllocatedTo { get; private set; }
    public ShippingAddress ShippingAddress { get; set; }
    public CustomerInformation CustomerInformation { get; set; }
    public IList<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    public DateTime? ExpiredAt { get; set; }

    public Order() { }

    public Order(string id, string site, string expectedCarrier, DateTimeOffset placedOn)
    {
        Id = id;
        Apply(new OrderCreated { Site = site, ExpectedCarrier = expectedCarrier, PlacedOn = placedOn });
    }

    public void SetCustomerInformation(string title, string name, string searchableName)
    {
        Apply(new CustomerInformationUpdated { Title = title, Name = name, SearchableName = searchableName });
    }

    public void AddOrderLine(OrderLine.OrderLineType type, string reference, string externalId, string sku, int quantity, DateTimeOffset? deliveryDeadline)
    {
        if(OrderLines.Any(l => l.Reference == reference)) throw new BadRequestException("Duplicate OrderLine reference");

        Apply(new OrderLineAdded
        {
            Type = type,
            Status = OrderLine.OrderLineStatus.Dispatched,
            Reference = reference,
            ExternalId = externalId,
            Sku = sku,
            Quantity = quantity,
            DeliveryDeadline = deliveryDeadline
        });
    }

    public void Allocate(string username)
    {
        if (!string.IsNullOrEmpty(AllocatedTo)) throw new BadRequestException("Order already allocated");

        Apply(new OrderAllocatedToUser { Username = username, AllocatedOn = DateTime.UtcNow });
    }

    public void Release(string username)
    {
        if (AllocatedTo != username) throw new BadRequestException("Order not allocated to user");

        Apply(new OrderReleasedFromUser { Username = username, ReleasedOn = DateTime.UtcNow });
    }

    internal void HandleEvent(CustomerInformationUpdated evt)
    {
        CustomerInformation = new()
        {
            Title = evt.Title,
            Name = evt.Name,
            SearchableName = evt.SearchableName
        };
    }

    internal void HandleEvent(OrderCreated evt)
    {
        Site = evt.Site;
        ExpectedCarrier = evt.ExpectedCarrier;
        PlacedOn = evt.PlacedOn;
    }

    internal void HandleEvent(OrderLineAdded evt)
    {
        OrderLines.Add(new OrderLine
        {
            Type = evt.Type,
            ExternalId = evt.ExternalId,
            Reference = evt.Reference,
            Sku = evt.Sku,
            Quantity = evt.Quantity,
            Status = evt.Status,
            DeliveryDateLine = evt.DeliveryDeadline,
        });
    }

    internal void HandleEvent(OrderAllocatedToUser evt)
    {
        AllocatedTo = evt.Username;
    }

    internal void HandleEvent(OrderReleasedFromUser _)
    {
        AllocatedTo = null;
    }
}
