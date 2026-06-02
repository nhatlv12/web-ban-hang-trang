using App.Trang.Api.Entities;
using App.Trang.Api.Features.Orders.Commands;
using App.Trang.Api.Features.Orders.Queries;
using MediatR;

namespace App.Trang.Api.Features.Orders;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapGet("/", async (OrderType? type, ISender sender) =>
        {
            var result = await sender.Send(new GetOrdersQuery(type));
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetOrderByIdQuery(id));
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        });

        group.MapPost("/", async (CreateOrderCommand cmd, ISender sender) =>
        {
            var result = await sender.Send(cmd);
            return result.Success
                ? Results.Created($"/api/orders/{result.Data}", result)
                : Results.BadRequest(result);
        });

        group.MapPut("/{id:guid}/status", async (Guid id, UpdateOrderStatusCommand cmd, ISender sender) =>
        {
            if (id != cmd.Id)
                return Results.BadRequest("Id không khớp.");

            var result = await sender.Send(cmd);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        group.MapPut("/{id:guid}/cancel", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new CancelOrderCommand(id));
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });
    }
}
