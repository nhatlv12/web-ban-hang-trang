using App.Trang.Api.Features.Products.Commands;
using App.Trang.Api.Features.Products.Queries;
using MediatR;

namespace App.Trang.Api.Features.Products;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetProductsQuery());
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProductByIdQuery(id));
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        });

        group.MapPost("/", async (CreateProductCommand cmd, ISender sender) =>
        {
            var result = await sender.Send(cmd);
            return result.Success
                ? Results.Created($"/api/products/{result.Data}", result)
                : Results.BadRequest(result);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateProductCommand cmd, ISender sender) =>
        {
            if (id != cmd.Id)
                return Results.BadRequest("Id không khớp.");

            var result = await sender.Send(cmd);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteProductCommand(id));
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        });
    }
}
