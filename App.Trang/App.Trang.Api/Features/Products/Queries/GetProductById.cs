using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

public class GetProductByIdHandler(AppDbContext db) : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.WareHouse)
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductDto(
                p.Id, p.Code, p.Name, p.Description,
                p.CategoryId, p.Category.Name,
                p.CostPrice, p.SellingPrice, p.OriginalPrice,
                p.Unit, p.Image, p.IsNew, p.IsSale, p.IsActive,
                p.WareHouse != null ? p.WareHouse.Quantity : 0,
                p.CreatedAt, p.UpdatedAt
            ))
            .FirstOrDefaultAsync(ct);

        return product is null
            ? Result<ProductDto>.Fail("Không tìm thấy sản phẩm.")
            : Result<ProductDto>.Ok(product);
    }
}
