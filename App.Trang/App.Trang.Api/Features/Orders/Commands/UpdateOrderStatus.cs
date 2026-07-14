using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Orders.Commands;

public record UpdateOrderStatusCommand(Guid Id, OrderStatus Status) : IRequest<Result>;

public class UpdateOrderStatusHandler(AppDbContext db) : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == request.Id, ct);

        if (order is null)
            return Result.Fail("Không tìm thấy đơn hàng.");

        if (order.Status == OrderStatus.Cancelled)
            return Result.Fail("Không thể thay đổi trạng thái đơn hàng đã hủy.");

        if (order.Status == OrderStatus.Completed && request.Status != OrderStatus.Cancelled)
            return Result.Fail("Đơn hàng đã hoàn thành, chỉ có thể hủy.");

        if (request.Status == OrderStatus.Cancelled)
        {
            foreach (var detail in order.OrderDetails)
            {
                var providerIdToUse = detail.ProviderId ?? order.ProviderId;
                var wareHouse = await db.WareHouses.FirstOrDefaultAsync(w => w.ProductId == detail.ProductId && w.ProviderId == providerIdToUse, ct);
                if (wareHouse == null)
                {
                    wareHouse = new WareHouse { ProductId = detail.ProductId, ProviderId = providerIdToUse, Quantity = 0 };
                    db.WareHouses.Add(wareHouse);
                }

                if (order.Type == OrderType.Export)
                {
                    wareHouse.Quantity += detail.Quantity;
                    wareHouse.TotalExport -= detail.Quantity; // Rollback export
                }
                else if (order.Type == OrderType.Import)
                {
                    wareHouse.Quantity -= detail.Quantity;
                    wareHouse.TotalImport -= detail.Quantity; // Rollback import
                }
                wareHouse.LastStockUpdate = DateTime.UtcNow;
            }
        }
        order.Status = request.Status;
        await db.SaveChangesAsync(ct);

        return Result.Ok("Cập nhật trạng thái đơn hàng thành công.");
    }
}
