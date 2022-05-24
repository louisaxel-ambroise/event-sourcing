namespace EventSourcing.MVP.Domain.Orders.Commands;

public record AllocateOrder(string OrderId, string Username);
