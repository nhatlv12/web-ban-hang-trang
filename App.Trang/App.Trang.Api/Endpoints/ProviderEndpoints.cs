using App.Trang.Api.Common.Models;
using App.Trang.Api.Features.Providers.Commands;
using App.Trang.Api.Features.Providers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Trang.Api.Endpoints;

public static class ProviderEndpoints
{
    public static void MapProviderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/providers").WithTags("Providers");

        group.MapGet("/", GetProviders)
             .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetProviderById)
             .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/", CreateProvider)
             .Produces<ApiResponse<Guid>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", UpdateProvider)
             .Produces<ApiResponse>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", DeleteProvider)
             .Produces<ApiResponse>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetProviders(IMediator mediator)
    {
        var result = await mediator.Send(new GetProvidersQuery());
        var apiResponse = new ApiResponse<object>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> GetProviderById([FromRoute] Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetProviderByIdQuery(id));
        var apiResponse = new ApiResponse<object>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> CreateProvider([FromBody] CreateProviderCommand cmd, IMediator mediator)
    {
        var result = await mediator.Send(cmd);
        var apiResponse = new ApiResponse<Guid>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> UpdateProvider([FromRoute] Guid id, [FromBody] UpdateProviderCommand cmd, IMediator mediator)
    {
        if (id != cmd.Id)
            return Results.BadRequest(new ApiResponse(false, "Id không khớp."));

        var result = await mediator.Send(cmd);
        var apiResponse = new ApiResponse(result.Success, result.Message);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> DeleteProvider([FromRoute] Guid id, ISender sender)
    {
        var result = await sender.Send(new DeleteProviderCommand(id));
        var apiResponse = new ApiResponse(result.Success, result.Message);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }
}
