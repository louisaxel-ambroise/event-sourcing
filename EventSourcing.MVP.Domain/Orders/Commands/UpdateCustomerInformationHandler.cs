using EventSourcing.MVP.Infrastructure.Store;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Commands;

public class UpdateCustomerInformationHandler
{
    private readonly Repository _repository;

    public UpdateCustomerInformationHandler(Repository repository) => _repository = repository;

    public async Task HandleAsync(UpdateCustomerInformation command, CancellationToken cancellationToken)
    {
        var order = await _repository.LoadAsync<Order>(command.OrderId, cancellationToken);
        order.SetCustomerInformation(command.Title, command.Name, command.SearchableName);

        await _repository.SaveAsync(order, cancellationToken);
    }
}
