using App.Trang.Api.Features.Categories.Commands;
using App.Trang.Api.Features.Categories.Queries;
using MediatR;

namespace App.Trang.Api.Features.Categories;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetCategoriesQuery());
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetCategoryByIdQuery(id));
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        });

        group.MapPost("/", async (CreateCategoryCommand cmd, ISender sender) =>
        {
            var result = await sender.Send(cmd);
            return result.Success
                ? Results.Created($"/api/categories/{result.Data}", result)
                : Results.BadRequest(result);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateCategoryCommand cmd, ISender sender) =>
        {
            if (id != cmd.Id)
                return Results.BadRequest("Id không khớp.");

            var result = await sender.Send(cmd);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteCategoryCommand(id));
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        });
    }
}
