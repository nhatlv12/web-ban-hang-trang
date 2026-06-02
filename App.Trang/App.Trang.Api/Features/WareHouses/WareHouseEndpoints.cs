using App.Trang.Api.Features.WareHouses.Commands;
using App.Trang.Api.Features.WareHouses.Queries;
using MediatR;

namespace App.Trang.Api.Features.WareHouses;

public static class WareHouseEndpoints
{
    public static void MapWareHouseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/warehouses").WithTags("WareHouses");

        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetWareHousesQuery());
            return Results.Ok(result);
        });

        group.MapGet("/product/{productId:guid}", async (Guid productId, ISender sender) =>
        {
            var result = await sender.Send(new GetWareHouseByProductIdQuery(productId));
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateStockCommand cmd, ISender sender) =>
        {
            if (id != cmd.Id)
                return Results.BadRequest("Id không khớp.");

            var result = await sender.Send(cmd);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });
    }
}
