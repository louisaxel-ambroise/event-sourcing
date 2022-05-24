namespace EventSourcing.MVP.Domain.Orders.Commands;

public record ReleaseOrder(string OrderId, string Username);
