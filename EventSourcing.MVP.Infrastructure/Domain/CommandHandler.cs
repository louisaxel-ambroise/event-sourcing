using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Infrastructure.Domain;

public abstract class CommandHandler<T>
{
    protected int MaximumRetryCount { get; set; } = 5;

    public Task ProcessAsync(T command, CancellationToken cancellationToken) => HandleAsync(command, cancellationToken);
    protected abstract Task HandleAsync(T command, CancellationToken cancellationToken);
}