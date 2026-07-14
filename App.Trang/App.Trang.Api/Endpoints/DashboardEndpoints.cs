using App.Trang.Api.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace App.Trang.Api.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboard").WithTags("Dashboard");

        group.MapGet("/", async (string? period, IMediator mediator) =>
        {
            return Results.Ok(new { Data = await mediator.Send(new GetDashboardStatsQuery(period)) });
        });
    }
}
