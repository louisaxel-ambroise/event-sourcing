namespace EventSourcing.MVP.Domain.Orders;

public class ShippingAddress
{
    public string Line1 { get; set; }
    public string Line2 { get; set; }
    public string Line3 { get; set; }
    public string City { get; set; }
    public string County { get; set; }
    public string Postcode { get; set; }
    public string CountryCode { get; set; }
}
