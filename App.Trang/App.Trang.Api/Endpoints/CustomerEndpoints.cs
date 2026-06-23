using App.Trang.Api.Common.Models;
using App.Trang.Api.Features.Customers.Commands;
using App.Trang.Api.Features.Customers.Queries;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;

namespace App.Trang.Api.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers").WithTags("Customers");

        group.MapGet("/", GetCustomers)
             .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetCustomerById)
             .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/", CreateCustomer)
             .Produces<ApiResponse<Guid>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", UpdateCustomer)
             .Produces<ApiResponse>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", DeleteCustomer)
             .Produces<ApiResponse>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetCustomers(IMediator mediator)
    {
        var result = await mediator.Send(new GetCustomersQuery());
        var apiResponse = new ApiResponse<object>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> GetCustomerById(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id));
        var apiResponse = new ApiResponse<object>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> CreateCustomer(CreateCustomerCommand cmd, IMediator mediator)
    {
        var result = await mediator.Send(cmd);
        var apiResponse = new ApiResponse<Guid>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> UpdateCustomer(Guid id, UpdateCustomerCommand cmd, IMediator mediator)
    {
        if (id != cmd.Id)
            return Results.BadRequest(new ApiResponse(false, "Id không khớp."));

        var result = await mediator.Send(cmd);
        var apiResponse = new ApiResponse(result.Success, result.Message);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> DeleteCustomer(Guid id, IMediator sender)
    {
        var result = await sender.Send(new DeleteCustomerCommand(id));
        var apiResponse = new ApiResponse(result.Success, result.Message);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }
}
