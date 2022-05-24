using EventSourcing.MVP.Domain.Orders;
using EventSourcing.MVP.Domain.Orders.Commands;
using EventSourcing.MVP.Domain.Orders.Consumers;
using EventSourcing.MVP.Infrastructure.Messaging;
using EventSourcing.MVP.Infrastructure.Store;
using EventSourcing.MVP.Postgresql;

namespace EventSourcing.MVP.Api.Modules;

public static class OrderModule
{
    private const string Prefix = "/orders";

    public static WebApplication MapOrdersEndpoints(this WebApplication app)
    {
        app.MapPost(Prefix, CreateOrder);
        app.MapPut(Prefix + "/{id}/customer", UpdateCustomerInformation);
        app.MapPut(Prefix + "/{id}/allocate", AllocateOrder);
        app.MapPut(Prefix + "/{id}/release", ReleaseOrder);

        return app;
    }

    public static WebApplicationBuilder RegisterOrdersModule(this WebApplicationBuilder builder)
    {
        EventSerializer.RegisterFromAssemblyContaining<Order>();
        var eventStore = new PostgresqlEventStore(builder.Configuration.GetConnectionString(nameof(PostgresqlEventStore)));

        builder.Services.AddHostedService(_ => new BackgroundEventConsumer<OrderProjection>(eventStore));
        builder.Services.AddHostedService(_ => new BackgroundEventConsumer<OrderStatusEmailReaction>(eventStore));
        builder.Services.AddSingleton(new Repository(eventStore));

        return builder;
    }

    static async Task<IResult> CreateOrder(CreateOrder command, Repository repository, CancellationToken cancellationToken)
    {
        var handler = new CreateOrderHandler(repository);
        await handler.HandleAsync(command, cancellationToken);

        return Results.Accepted($"/reservation/{command.Id}");
    }

    static async Task<IResult> UpdateCustomerInformation(string id, UpdateCustomerInformation command, Repository repository, CancellationToken cancellationToken)
    {
        var handler = new UpdateCustomerInformationHandler(repository);
        await handler.HandleAsync(command with { OrderId = id }, cancellationToken);

        return Results.NoContent();
    }

    static async Task<IResult> AllocateOrder(string id, AllocateOrder command, Repository repository, CancellationToken cancellationToken)
    {
        var handler = new AllocateOrderHandler(repository);
        await handler.HandleAsync(command with { OrderId = id }, cancellationToken);

        return Results.NoContent();
    }

    static async Task<IResult> ReleaseOrder(string id, ReleaseOrder command, Repository repository, CancellationToken cancellationToken)
    {
        var handler = new ReleaseOrderHandler(repository);
        await handler.HandleAsync(command with { OrderId = id }, cancellationToken);

        return Results.NoContent();
    }
}