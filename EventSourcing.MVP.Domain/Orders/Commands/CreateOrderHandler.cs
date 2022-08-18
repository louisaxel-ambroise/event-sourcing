using EventSourcing.MVP.Domain.Shared;
using EventSourcing.MVP.Infrastructure.Domain;
using EventSourcing.MVP.Infrastructure.Exceptions;
using EventSourcing.MVP.Infrastructure.Store;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Commands;

public class CreateOrderHandler : CommandHandler<CreateOrder>
{
    private readonly Repository<Order> _repository;
    private readonly SharedData _sharedData;

    public CreateOrderHandler(Repository<Order> repository, SharedData sharedData)
    {
        _repository = repository;
        _sharedData = sharedData;
    }

    protected override async Task HandleAsync(CreateOrder command, CancellationToken cancellationToken)
    {
        var site = _sharedData.LoadStoreByName(command.Site);

        if(site is null)
        {
            throw new BadRequestException("Site does not exist");
        }

        var order = new Order(command.Id, site.Id, command.ExpectedCarrier, command.PlacedOn);
        order.SetCustomerInformation(command.Customer.Title, command.Customer.Name, command.Customer.SearchableName);

        foreach (var line in command.OrderLines)
        {
            var type = Enum.Parse<OrderLine.OrderLineType>(line.Type);
            order.AddOrderLine(type, line.Reference, line.ExternalId, line.Sku, line.Quantity, line.DeliveryDeadline);
        }

        await _repository.SaveAsync(order, cancellationToken);
    }
}
