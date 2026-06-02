using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using MediatR;

namespace App.Trang.Api.Features.Orders.Commands;

public record UpdateOrderStatusCommand(Guid Id, OrderStatus Status) : IRequest<Result>;

public class UpdateOrderStatusHandler(AppDbContext db) : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await db.Orders.FindAsync([request.Id], ct);
        if (order is null)
            return Result.Fail("Không tìm thấy đơn hàng.");

        if (order.Status == OrderStatus.Cancelled)
            return Result.Fail("Không thể thay đổi trạng thái đơn hàng đã hủy.");

        if (order.Status == OrderStatus.Completed && request.Status != OrderStatus.Cancelled)
            return Result.Fail("Đơn hàng đã hoàn thành, chỉ có thể hủy.");

        order.Status = request.Status;
        await db.SaveChangesAsync(ct);

        return Result.Ok("Cập nhật trạng thái đơn hàng thành công.");
    }
}
