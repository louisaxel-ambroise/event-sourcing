using System;

namespace EventSourcing.MVP.Domain.Orders;

public class OrderLine
{
    public OrderLineType Type { get; set; }
    public string ExternalId { get; set; }
    public string Reference { get; set; }
    public string Sku { get; set; }
    public int Quantity { get; set; }
    public string Location { get; set; }
    public DateTimeOffset? DeliveryDateLine { get; set; }
    public OrderLineStatus Status { get; set; }

    public enum OrderLineType
    {
        Central,
        Local,
        StoreDispatch
    }

    public enum OrderLineStatus
    {
        Dispatched,
        Received,
        ToPick,
        PickInProgress,
        Picked,
        PutAwayInProgress,
        PutAway,
        OnHold,
        Cancelled,
        Collected,
        Expired,
        ReturnedToStock,
        PackInProgress,
        Packed,
        Shipped,
        Returned,
        Misdirected,
        Uncollected,
        Waiting,
        Closed
    }
}
