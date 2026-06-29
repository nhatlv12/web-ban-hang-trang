using App.Trang.Api.Common.Models;
using App.Trang.Api.Features.Products.Commands;
using App.Trang.Api.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Trang.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", GetProducts)
             .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetProductById)
             .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/", CreateProduct)
             .DisableAntiforgery()
             .Produces<ApiResponse<Guid>>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", UpdateProduct)
             .DisableAntiforgery()
             .Produces<ApiResponse>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", DeleteProduct)
             .Produces<ApiResponse>(StatusCodes.Status200OK)
             .Produces<ApiResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetProducts(IMediator mediator)
    {
        var result = await mediator.Send(new GetProductsQuery());
        var apiResponse = new ApiResponse<object>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> GetProductById([FromRoute] Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        var apiResponse = new ApiResponse<object>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> CreateProduct([FromForm] CreateProductCommand cmd, IMediator mediator)
    {
        var result = await mediator.Send(cmd);
        var apiResponse = new ApiResponse<Guid>(result.Success, result.Message, result.Data);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> UpdateProduct([FromRoute] Guid id, [FromForm] UpdateProductCommand cmd, IMediator mediator)
    {
        cmd.Id = id;
        var result = await mediator.Send(cmd);
        var apiResponse = new ApiResponse(result.Success, result.Message);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }

    private static async Task<IResult> DeleteProduct([FromRoute] Guid id, ISender sender)
    {
        var result = await sender.Send(new DeleteProductCommand(id));
        var apiResponse = new ApiResponse(result.Success, result.Message);
        return result.Success ? Results.Ok(apiResponse) : Results.BadRequest(apiResponse);
    }
}
