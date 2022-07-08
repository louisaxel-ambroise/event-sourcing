using EventSourcing.MVP.Infrastructure.Messaging;
using EventSourcing.MVP.Infrastructure.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.MVP.Infrastructure.Consumers;

public abstract class EventConsumer
{
    public const int NoEventsDelay = 500;
    public const int FillGapDelay = 200;
    public const int BatchSize = 1_000;

    private int _lastProcessedEvent;
    private readonly EventHandlerRegistry _handlers = new();

    public async virtual Task InitializeAsync(CancellationToken cancellationToken)
    {
        _lastProcessedEvent = await GetLastProcessedEventIdAsync(cancellationToken);
    }

    public async Task RunAsync(IEventStore eventStore, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var events = await GetProjectionableEvents(eventStore, stoppingToken);

                if (!events.Any())
                {
                    await Task.Delay(NoEventsDelay, stoppingToken);
                    continue;
                }

                await ProcessEventsAsync(events.Select(EventSerializer.Deserialize).Where(CanHandle), stoppingToken);
                _lastProcessedEvent = events.Max(x => x.Id);
            }
            catch
            {
                await Task.Delay(NoEventsDelay, stoppingToken); // Let SQL breathe before retry.. 
            }
        }
    }

    protected void Register<T>(Func<T, CancellationToken, Task> handler) where T : IEvent => _handlers.Register(handler);
    protected bool CanHandle<T>(T evt) where T : IEvent => _handlers.CanHandle(evt);
    protected Task HandleAsync<T>(T evt, CancellationToken cancellationToken) where T : IEvent => _handlers.HandleAsync(evt, cancellationToken);
    
    protected abstract Task<int> GetLastProcessedEventIdAsync(CancellationToken cancellationToken);
    protected abstract Task ProcessEventsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken);

    private async Task<IEnumerable<Event>> GetProjectionableEvents(IEventStore eventStore, CancellationToken cancellationToken)
    {
        IEnumerable<Event> events;

        do
        {
            events = await eventStore.ListEventsAsync(_lastProcessedEvent, BatchSize, cancellationToken);
        }
        while (ExistsEventGap(events) && await WaitForGapFill(cancellationToken));

        return events;
    }

    /// <summary>
    /// Checks if the specified list of events has a gap that can still be filled, i.e. some commands
    /// might still being processed but the events are not yet persisted in the event store.
    /// A valid event list has the following characteristics:
    ///  - It's the first time the projection is starts (lastProcessedId <0)
    ///    OR
    ///  - There is no event in the list
    ///    OR
    ///  - The last inserted event was written in the database more than 5 seconds ago.
    ///    OR
    ///  - All the events have an incremental gapless ID value that are consecutive to the last processed one
    /// </summary>
    /// <param name="events">The list of events to be validated</param>
    /// <returns>True if there is a possible fillable gap in the events</returns>
    private bool ExistsEventGap(IEnumerable<Event> events)
    {
        if (_lastProcessedEvent < 0 || !events.Any() || events.Max(x => x.InsertedAt).AddSeconds(5) < DateTime.UtcNow)
        {
            return false;
        }

        return events.Min(x => x.Id) > _lastProcessedEvent + 1 || events.Max(x => x.Id) - events.Min(x => x.Id) + 1 > events.Count();
    }

    /// <summary>
    /// Pause the projection for a given time to make sure no events are missed.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the projection is stopped</param>
    /// <returns>If the wait operation was successful</returns>
    private static Task<bool> WaitForGapFill(CancellationToken cancellationToken)
    {
        return Task.Delay(TimeSpan.FromMilliseconds(FillGapDelay), cancellationToken).ContinueWith(x => true);
    }
}
