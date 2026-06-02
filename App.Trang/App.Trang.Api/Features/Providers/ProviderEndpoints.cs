using App.Trang.Api.Features.Providers.Commands;
using App.Trang.Api.Features.Providers.Queries;
using MediatR;

namespace App.Trang.Api.Features.Providers;

public static class ProviderEndpoints
{
    public static void MapProviderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/providers").WithTags("Providers");

        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetProvidersQuery());
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProviderByIdQuery(id));
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        });

        group.MapPost("/", async (CreateProviderCommand cmd, ISender sender) =>
        {
            var result = await sender.Send(cmd);
            return result.Success
                ? Results.Created($"/api/providers/{result.Data}", result)
                : Results.BadRequest(result);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateProviderCommand cmd, ISender sender) =>
        {
            if (id != cmd.Id)
                return Results.BadRequest("Id không khớp.");

            var result = await sender.Send(cmd);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteProviderCommand(id));
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        });
    }
}
