using EventSourcing.MVP.Infrastructure.Consumers;
using EventSourcing.MVP.Infrastructure.Store;

namespace EventSourcing.MVP.Api;

public class BackgroundEventConsumer<TConsumer> : BackgroundService
    where TConsumer : EventConsumer, new()
{
    private readonly IEventStore _eventStore;
    private readonly EventConsumer _projection;

    public BackgroundEventConsumer(IEventStore eventStore)
    {
        _eventStore = eventStore;
        _projection = new TConsumer();
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _projection.InitializeAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() => _projection.RunAsync(_eventStore, stoppingToken), stoppingToken);

        return Task.CompletedTask;
    }
}
