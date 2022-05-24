using EventSourcing.MVP.Infrastructure.Store;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Commands;

public class ReleaseOrderHandler
{
    private readonly Repository _repository;

    public ReleaseOrderHandler(Repository repository) => _repository = repository;

    public async Task HandleAsync(ReleaseOrder command, CancellationToken cancellationToken)
    {
        var order = await _repository.LoadAsync<Order>(command.OrderId, cancellationToken);
        order.Release(command.Username);

        await _repository.SaveAsync(order, cancellationToken);
    }
}
