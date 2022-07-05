﻿using EventSourcing.MVP.Infrastructure.Store;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Domain.Orders.Commands;

public class AllocateOrderHandler
{
    private readonly Repository<Order> _repository;

    public AllocateOrderHandler(Repository<Order> repository) => _repository = repository;

    public async Task HandleAsync(AllocateOrder command, CancellationToken cancellationToken)
    {
        var order = await _repository.LoadAsync(command.OrderId, cancellationToken);
        order.Allocate(command.Username);

        await _repository.SaveAsync(order, cancellationToken);
    }
}
