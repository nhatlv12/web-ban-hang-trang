using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.WareHouses.Queries;

public record WareHouseDto(
    Guid Id, Guid ProductId, string ProductCode, string ProductName,
    int Quantity, int MinQuantity, int MaxQuantity,
    string? Location, DateTime LastStockUpdate,
    DateTime CreatedAt, DateTime? UpdatedAt
);

public record GetWareHousesQuery() : IRequest<Result<List<WareHouseDto>>>;

public class GetWareHousesHandler(AppDbContext db) : IRequestHandler<GetWareHousesQuery, Result<List<WareHouseDto>>>
{
    public async Task<Result<List<WareHouseDto>>> Handle(GetWareHousesQuery request, CancellationToken ct)
    {
        var wareHouses = await db.WareHouses
            .AsNoTracking()
            .Include(w => w.Product)
            .OrderBy(w => w.Product.Name)
            .Select(w => new WareHouseDto(
                w.Id, w.ProductId, w.Product.Code, w.Product.Name,
                w.Quantity, w.MinQuantity, w.MaxQuantity,
                w.Location, w.LastStockUpdate,
                w.CreatedAt, w.UpdatedAt
            ))
            .ToListAsync(ct);

        return Result<List<WareHouseDto>>.Ok(wareHouses);
    }
}
