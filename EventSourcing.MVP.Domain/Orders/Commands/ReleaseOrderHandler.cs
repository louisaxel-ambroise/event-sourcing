using EventSourcing.MVP.Infrastructure.Store;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Commands;

public class ReleaseOrderHandler
{
    private readonly Repository<Order> _repository;

    public ReleaseOrderHandler(Repository<Order> repository) => _repository = repository;

    public async Task HandleAsync(ReleaseOrder command, CancellationToken cancellationToken)
    {
        var order = await _repository.LoadAsync(command.OrderId, cancellationToken);
        order.Release(command.Username);

        await _repository.SaveAsync(order, cancellationToken);
    }
}
