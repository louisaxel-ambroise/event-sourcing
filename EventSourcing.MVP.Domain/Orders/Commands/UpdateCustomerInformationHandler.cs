using EventSourcing.MVP.Infrastructure.Store;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Commands;

public class UpdateCustomerInformationHandler
{
    private readonly Repository<Order> _repository;

    public UpdateCustomerInformationHandler(Repository<Order> repository) => _repository = repository;

    public async Task HandleAsync(UpdateCustomerInformation command, CancellationToken cancellationToken)
    {
        var order = await _repository.LoadAsync(command.OrderId, cancellationToken);
        order.SetCustomerInformation(command.Title, command.Name, command.SearchableName);

        await _repository.SaveAsync(order, cancellationToken);
    }
}
