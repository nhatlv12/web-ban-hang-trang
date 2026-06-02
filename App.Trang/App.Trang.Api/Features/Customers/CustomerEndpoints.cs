using App.Trang.Api.Features.Customers.Commands;
using App.Trang.Api.Features.Customers.Queries;
using MediatR;

namespace App.Trang.Api.Features.Customers;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers").WithTags("Customers");

        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetCustomersQuery());
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetCustomerByIdQuery(id));
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        });

        group.MapPost("/", async (CreateCustomerCommand cmd, ISender sender) =>
        {
            var result = await sender.Send(cmd);
            return result.Success
                ? Results.Created($"/api/customers/{result.Data}", result)
                : Results.BadRequest(result);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateCustomerCommand cmd, ISender sender) =>
        {
            if (id != cmd.Id)
                return Results.BadRequest("Id không khớp.");

            var result = await sender.Send(cmd);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteCustomerCommand(id));
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        });
    }
}
