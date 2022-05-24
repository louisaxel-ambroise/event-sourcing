namespace EventSourcing.MVP.Domain.Orders.Commands;

public record UpdateCustomerInformation(string OrderId, string Title, string Name, string SearchableName);
