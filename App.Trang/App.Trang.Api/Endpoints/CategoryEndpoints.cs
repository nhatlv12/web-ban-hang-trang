using App.Trang.Api.Common.Models;
using App.Trang.Api.Features.Categories.Commands;
using App.Trang.Api.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Trang.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", GetCategories)
             .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetCategoryById)
             .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/", CreateCategory)
             .Produces<ApiResponse<Guid>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", UpdateCategory)
             .Produces<ApiResponse>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", DeleteCategory)
             .Produces<ApiResponse>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetCategories(IMediator mediator)
    {
        var result = await mediator.Send(new GetCategoriesQuery());
        var apiResponse = new ApiResponse<object>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> GetCategoryById([FromRoute] Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetCategoryByIdQuery(id));
        var apiResponse = new ApiResponse<object>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> CreateCategory([FromBody] CreateCategoryCommand cmd, IMediator mediator)
    {
        var result = await mediator.Send(cmd);
        var apiResponse = new ApiResponse<Guid>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryCommand cmd, IMediator mediator)
    {
        if (id != cmd.Id)
            return Results.BadRequest(new ApiResponse(false, "Id không khớp."));

        var result = await mediator.Send(cmd);
        var apiResponse = new ApiResponse(result.Success, result.Message);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> DeleteCategory([FromRoute] Guid id, ISender sender)
    {
        var result = await sender.Send(new DeleteCategoryCommand(id));
        var apiResponse = new ApiResponse(result.Success, result.Message);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }
}
