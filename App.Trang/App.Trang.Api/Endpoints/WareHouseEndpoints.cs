using App.Trang.Api.Common.Models;
using App.Trang.Api.Features.WareHouses.Commands;
using App.Trang.Api.Features.WareHouses.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Trang.Api.Endpoints;

public static class WareHouseEndpoints
{
    public static void MapWareHouseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/warehouses").WithTags("WareHouses");

        group.MapGet("/", GetWareHouses)
             .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/product/{productId:guid}", GetWareHouseByProductId)
             .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", UpdateStock)
             .Produces<ApiResponse>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetWareHouses(IMediator mediator)
    {
        var result = await mediator.Send(new GetWareHousesQuery());
        var apiResponse = new ApiResponse<object>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> GetWareHouseByProductId([FromRoute] Guid productId, IMediator mediator)
    {
        var result = await mediator.Send(new GetWareHouseByProductIdQuery(productId));
        var apiResponse = new ApiResponse<object>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> UpdateStock([FromRoute] Guid id, [FromBody] UpdateStockCommand cmd, IMediator mediator)
    {
        if (id != cmd.Id)
            return Results.BadRequest(new ApiResponse(false, "Id không khớp."));

        var result = await mediator.Send(cmd);
        var apiResponse = new ApiResponse(result.Success, result.Message);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }
}
