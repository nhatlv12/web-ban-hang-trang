using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.WareHouses.Queries;

public record GetWareHouseByProductIdQuery(Guid ProductId) : IRequest<Result<WareHouseDto>>;

public class GetWareHouseByProductIdHandler(AppDbContext db)
    : IRequestHandler<GetWareHouseByProductIdQuery, Result<WareHouseDto>>
{
    public async Task<Result<WareHouseDto>> Handle(GetWareHouseByProductIdQuery request, CancellationToken ct)
    {
        var wareHouse = await db.WareHouses
            .AsNoTracking()
            .Include(w => w.Product)
            .Include(w => w.Provider)
            .Where(w => w.ProductId == request.ProductId)
            .Select(w => new WareHouseDto(
                w.Id, w.ProductId, w.Product.Code, w.Product.Name,
                w.ProviderId, w.Provider != null ? w.Provider.Name : null,
                w.Quantity, w.MinQuantity, w.MaxQuantity,
                w.Location, w.LastStockUpdate,
                w.CreatedAt, w.UpdatedAt
            ))
            .FirstOrDefaultAsync(ct);

        return wareHouse is null
            ? Result<WareHouseDto>.Fail("Không tìm thấy tồn kho cho sản phẩm này.")
            : Result<WareHouseDto>.Ok(wareHouse);
    }
}
