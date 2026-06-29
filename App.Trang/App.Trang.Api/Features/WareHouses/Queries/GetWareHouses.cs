using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.WareHouses.Queries;

public record WareHouseDto(
    Guid Id, Guid ProductId, string ProductCode, string ProductName,
    Guid? ProviderId, string? ProviderName,
    int Quantity, int MinQuantity, int MaxQuantity,
    int TotalImport, int TotalExport,
    string? Location, decimal CostPrice, DateTime? ImportDate,
    DateTime LastStockUpdate,
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
            .Include(w => w.Provider)
            .OrderBy(w => w.Product.Name)
            .Select(w => new WareHouseDto(
                w.Id, w.ProductId, w.Product.Code, w.Product.Name,
                w.ProviderId, w.Provider != null ? w.Provider.Name : null,
                w.Quantity, w.MinQuantity, w.MaxQuantity,
                w.TotalImport, w.TotalExport,
                w.Location, w.CostPrice, w.ImportDate,
                w.LastStockUpdate,
                w.CreatedAt, w.UpdatedAt
            ))
            .ToListAsync(ct);

        return Result<List<WareHouseDto>>.Ok(wareHouses);
    }
}
