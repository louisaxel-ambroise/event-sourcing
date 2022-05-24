using EventSourcing.MVP.Infrastructure.Store;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Commands;

public class AllocateOrderHandler
{
    private readonly Repository _repository;

    public AllocateOrderHandler(Repository repository) => _repository = repository;

    public async Task HandleAsync(AllocateOrder command, CancellationToken cancellationToken)
    {
        var order = await _repository.LoadAsync<Order>(command.OrderId, cancellationToken);
        order.Allocate(command.Username);

        await _repository.SaveAsync(order, cancellationToken);
    }
}
