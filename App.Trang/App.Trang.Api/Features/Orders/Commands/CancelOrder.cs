using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Orders.Commands;

public record CancelOrderCommand(Guid Id) : IRequest<Result>;

public class CancelOrderHandler(AppDbContext db) : IRequestHandler<CancelOrderCommand, Result>
{
    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == request.Id, ct);

        if (order is null)
            return Result.Fail("Không tìm thấy đơn hàng.");

        if (order.Status == OrderStatus.Cancelled)
            return Result.Fail("Đơn hàng đã được hủy trước đó.");

        // Hoàn lại tồn kho
        foreach (var detail in order.OrderDetails)
        {
            var wareHouse = await db.WareHouses
                .FirstOrDefaultAsync(w => w.ProductId == detail.ProductId, ct);

            if (wareHouse != null)
            {
                if (order.Type == OrderType.Import)
                    wareHouse.Quantity -= detail.Quantity; // Hoàn lại nhập
                else
                    wareHouse.Quantity += detail.Quantity; // Hoàn lại xuất

                wareHouse.LastStockUpdate = DateTime.UtcNow;
            }
        }

        order.Status = OrderStatus.Cancelled;
        await db.SaveChangesAsync(ct);

        return Result.Ok("Hủy đơn hàng và hoàn lại tồn kho thành công.");
    }
}
