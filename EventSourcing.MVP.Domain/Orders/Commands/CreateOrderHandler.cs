using EventSourcing.MVP.Infrastructure.Store;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Commands;

public class CreateOrderHandler
{
    private readonly Repository _repository;

    public CreateOrderHandler(Repository repository) => _repository = repository;

    public async Task HandleAsync(CreateOrder command, CancellationToken cancellationToken)
    {
        var order = new Order(command.Id, command.Site, command.ExpectedCarrier, command.PlacedOn);
        order.SetCustomerInformation(command.Customer.Title, command.Customer.Name, command.Customer.SearchableName);

        foreach (var line in command.OrderLines)
        {
            var type = Enum.Parse<OrderLine.OrderLineType>(line.Type);
            order.AddOrderLine(type, line.Reference, line.ExternalId, line.Sku, line.Quantity, line.DeliveryDeadline);
        }

        await _repository.SaveAsync(order, cancellationToken);
    }
}
