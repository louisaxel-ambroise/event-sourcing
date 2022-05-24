using System;
using System.Collections.Generic;

namespace EventSourcing.MVP.Domain.Orders.Commands;

public class CreateOrder
{
    public string Id { get; set; }
    public string Site { get; set; }
    public string ExpectedCarrier { get; set; }
    public DateTimeOffset PlacedOn { get; set; }
    public CustomerInformation Customer { get; set; }
    public IEnumerable<OrderLine> OrderLines { get; set; }

    public class OrderLine
    {
        public string Type { get; set; }
        public string ExternalId { get; set; }
        public string Reference { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public DateTimeOffset? DeliveryDeadline { get; set; }
    }

    public class CustomerInformation
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string SearchableName { get; set; }
    }
}
