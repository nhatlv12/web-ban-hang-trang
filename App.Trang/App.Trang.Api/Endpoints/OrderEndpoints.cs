using App.Trang.Api.Common.Models;
using App.Trang.Api.Entities;
using App.Trang.Api.Features.Orders.Commands;
using App.Trang.Api.Features.Orders.Queries;
using MediatR;

namespace App.Trang.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapGet("/", GetOrders)
             .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetOrderById)
             .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/", CreateOrder)
             .Produces<ApiResponse<Guid>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}/status", UpdateOrderStatus)
             .Produces<ApiResponse>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}/cancel", CancelOrder)
             .Produces<ApiResponse>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetOrders(OrderType? type, IMediator mediator)
    {
        var result = await mediator.Send(new GetOrdersQuery(type));
        var apiResponse = new ApiResponse<object>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> GetOrderById(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(id));
        var apiResponse = new ApiResponse<object>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> CreateOrder(CreateOrderCommand cmd, IMediator mediator)
    {
        var result = await mediator.Send(cmd);
        var apiResponse = new ApiResponse<Guid>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> UpdateOrderStatus(Guid id, UpdateOrderStatusCommand cmd, IMediator mediator)
    {
        if (id != cmd.Id)
            return Results.BadRequest(new ApiResponse(false, "Id không khớp."));

        var result = await mediator.Send(cmd);
        var apiResponse = new ApiResponse(result.Success, result.Message);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> CancelOrder(Guid id, ISender sender)
    {
        var result = await sender.Send(new CancelOrderCommand(id));
        var apiResponse = new ApiResponse(result.Success, result.Message);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }
}
