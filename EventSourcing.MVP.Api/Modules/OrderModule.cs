using EventSourcing.MVP.Domain.Orders;
using EventSourcing.MVP.Domain.Orders.Commands;
using EventSourcing.MVP.Domain.Orders.Consumers;
using EventSourcing.MVP.Domain.Shared;
using EventSourcing.MVP.Infrastructure.Store;

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

        builder.Services.AddHostedService<BackgroundEventConsumer<OrderProjection>>();
        builder.Services.AddHostedService<BackgroundEventConsumer<OrderStatusEmailReaction>>();
        builder.Services.AddSingleton<Repository<Order>>();

        return builder;
    }

    static async Task<IResult> CreateOrder(CreateOrder command, Repository<Order> repository, SharedData sharedData, CancellationToken cancellationToken)
    {
        var handler = new CreateOrderHandler(repository, sharedData);
        await handler.ProcessAsync(command, cancellationToken);

        return Results.Accepted($"/reservation/{command.Id}");
    }

    static async Task<IResult> UpdateCustomerInformation(string id, UpdateCustomerInformation command, Repository<Order> repository, CancellationToken cancellationToken)
    {
        var handler = new UpdateCustomerInformationHandler(repository);
        await handler.ProcessAsync(command with { OrderId = id }, cancellationToken);

        return Results.NoContent();
    }

    static async Task<IResult> AllocateOrder(string id, AllocateOrder command, Repository<Order> repository, CancellationToken cancellationToken)
    {
        var handler = new AllocateOrderHandler(repository);
        await handler.ProcessAsync(command with { OrderId = id }, cancellationToken);

        return Results.NoContent();
    }

    static async Task<IResult> ReleaseOrder(string id, ReleaseOrder command, Repository<Order> repository, CancellationToken cancellationToken)
    {
        var handler = new ReleaseOrderHandler(repository);
        await handler.ProcessAsync(command with { OrderId = id }, cancellationToken);

        return Results.NoContent();
    }
}