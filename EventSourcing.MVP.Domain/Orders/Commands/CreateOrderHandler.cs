using EventSourcing.MVP.Infrastructure.Domain;
using EventSourcing.MVP.Infrastructure.Store;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Commands;

public class CreateOrderHandler : CommandHandler<CreateOrder>
{
    private readonly Repository<Order> _repository;

    public CreateOrderHandler(Repository<Order> repository) => _repository = repository;

    protected override async Task HandleAsync(CreateOrder command, CancellationToken cancellationToken)
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
