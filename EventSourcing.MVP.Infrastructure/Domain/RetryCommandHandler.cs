using EventSourcing.MVP.Infrastructure.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Infrastructure.Domain;

public abstract class RetryCommandHandler<T>
{
    protected int MaximumRetryCount { get; set; } = 5;

    public async Task ProcessAsync(T command, CancellationToken cancellationToken)
    {
        var retryCount = 0;

        do
        {
            try
            {
                await HandleAsync(command, cancellationToken);
                break;
            }
            catch (ConcurrencyAggregateException)
            {
                retryCount++;
            }
        }
        while (!cancellationToken.IsCancellationRequested && retryCount < MaximumRetryCount);
    }

    protected abstract Task HandleAsync(T command, CancellationToken cancellationToken);
}
