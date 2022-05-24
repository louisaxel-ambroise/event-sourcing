using EventSourcing.MVP.Infrastructure.Messaging;

namespace EventSourcing.MVP.Domain.Orders.Events;

public class CustomerInformationUpdated : IEvent
{
    public string Title { get; set; }
    public string Name { get; set; }
    public string SearchableName { get; set; }
}
