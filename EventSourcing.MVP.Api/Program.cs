using EventSourcing.MVP.Api.Modules;
using EventSourcing.MVP.Infrastructure.Exceptions;
using EventSourcing.MVP.Infrastructure.Store;
using EventSourcing.MVP.Postgresql;
using Microsoft.AspNetCore.Diagnostics;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
var eventStore = new PostgresqlEventStore(builder.Configuration.GetConnectionString(nameof(PostgresqlEventStore)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IEventStore>(eventStore);
builder.RegisterOrdersModule();

// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseSwagger();
app.UseHttpsRedirection();
app.MapOrdersEndpoints();
app.UseExceptionHandler(h => h.Run(ctx => 
    {
        var exceptionHandlerPathFeature = ctx.Features.Get<IExceptionHandlerPathFeature>();
        var error = exceptionHandlerPathFeature.Error?.InnerException ?? exceptionHandlerPathFeature.Error;

        var (statusCode, message) = error switch
        {
            ConcurrencyAggregateException => (409, "A concurrency exception happened"),
            MissingAggregateException => (404, "Aggregate not found"),
            BadRequestException => (400, "Invalid request"),
            _ => (500, "An unexpected error occured")
        };

        ctx.Response.StatusCode = statusCode;
        ctx.Response.WriteAsJsonAsync(new { message });

        return Task.CompletedTask;
    })
);

app.Run();
