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
    public bool IsReadyToCollect { get; set; }

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

    public void Allocate(string allocatedTo)
    {
        if (!string.IsNullOrEmpty(AllocatedTo)) throw new BadRequestException("Order already allocated");

        Apply(new OrderAllocatedToUser { AllocatedTo = allocatedTo, AllocatedOn = DateTime.UtcNow });
    }

    public void Release(string releasedFrom)
    {
        if (AllocatedTo != releasedFrom) throw new BadRequestException("Order not allocated to user");

        Apply(new OrderReleasedFromUser { ReleasedFrom = releasedFrom, ReleasedOn = DateTime.UtcNow });
    }

    public void Expire(DateTime expiredAt, string expiredBy)
    {
        if (ExpiredAt.HasValue) throw new BadRequestException("Order already expired");

        Apply(new OrderExpired { ExpiredAt = expiredAt, ExpiredBy = expiredBy });
    }

    public void Collect()
    {
        if(!IsReadyToCollect) throw new BadRequestException("Order not ready to be collected");
        if(!OrderLines.Any(x => x.CanBeCollected())) throw new BadRequestException("Not valid for collection");

        var events = OrderLines
            .Where(x => x.CanBeCollected())
            .Select(line => new OrderLineCollected { Reference = line.Reference, CollectedAt = DateTime.UtcNow })
            .ToArray();

        Apply(events);
    }

    internal void Handle(CustomerInformationUpdated evt)
    {
        CustomerInformation = new()
        {
            Title = evt.Title,
            Name = evt.Name,
            SearchableName = evt.SearchableName
        };
    }

    internal void Handle(OrderCreated evt)
    {
        Site = evt.Site;
        ExpectedCarrier = evt.ExpectedCarrier;
        PlacedOn = evt.PlacedOn;
    }

    internal void Handle(OrderLineAdded evt)
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

    internal void Handle(OrderAllocatedToUser evt)
    {
        AllocatedTo = evt.AllocatedTo;
    }

    internal void Handle(OrderReleasedFromUser _)
    {
        AllocatedTo = null;
    }

    internal void Handle(OrderExpired evt)
    {
        ExpiredAt = evt.ExpiredAt;
    }

    internal void Handle(OrderLineCollected evt)
    {
        OrderLines
            .Single(x => x.Reference == evt.Reference)
            .Collected();
    }
}
