using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Products.Queries;

public record ProductDto(
    Guid Id, string Code, string Name, string? Description,
    Guid CategoryId, string CategoryName,
    decimal CostPrice, decimal SellingPrice, decimal? OriginalPrice,
    string Unit, string? Image, bool IsNew, bool IsSale, bool IsActive,
    int StockQuantity,
    DateTime CreatedAt, DateTime? UpdatedAt
);

public record GetProductsQuery() : IRequest<Result<List<ProductDto>>>;

public class GetProductsHandler(AppDbContext db) : IRequestHandler<GetProductsQuery, Result<List<ProductDto>>>
{
    public async Task<Result<List<ProductDto>>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var products = await db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.WareHouses)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProductDto(
                p.Id, p.Code, p.Name, p.Description,
                p.CategoryId, p.Category.Name,
                p.CostPrice, p.SellingPrice, p.OriginalPrice,
                p.Unit, p.Image, p.IsNew, p.IsSale, p.IsActive,
                p.WareHouses.Sum(w => w.Quantity),
                p.CreatedAt, p.UpdatedAt
            ))
            .ToListAsync(ct);

        return Result<List<ProductDto>>.Ok(products);
    }
}
