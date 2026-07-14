using System.Text.Json.Serialization;
using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Orders.Commands;

// === DTO cho chi tiết đơn hàng khi cập nhật ===
public record UpdateOrderDetailDto(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal Discount,
    decimal Tax,
    Guid? ProviderId
);

// === Command ===
public record UpdateOrderCommand(
    string Code,
    OrderType Type,
    DateTime OrderDate,
    Guid? ProviderId,
    Guid? CustomerId,
    decimal Discount,
    decimal ShippingFee,
    string? Note,
    List<UpdateOrderDetailDto> Details
) : IRequest<Result<Guid>>
{
    [JsonIgnore]
    public Guid Id { get; set; }
}

// === Validator ===
public class UpdateOrderValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderValidator()
    {
        RuleFor(x => x.Code).MaximumLength(20);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.OrderDate).NotEmpty();
        RuleFor(x => x.Details).NotEmpty().WithMessage("Đơn hàng phải có ít nhất một chi tiết.");
        RuleForEach(x => x.Details).ChildRules(detail =>
        {
            detail.RuleFor(d => d.ProductId).NotEmpty();
            detail.RuleFor(d => d.Quantity).GreaterThan(0);
            detail.RuleFor(d => d.UnitPrice).GreaterThanOrEqualTo(0);
            detail.RuleFor(d => d.Discount).GreaterThanOrEqualTo(0);
        });

        RuleFor(x => x.CustomerId)
            .NotEmpty().When(x => x.Type == OrderType.Export)
            .WithMessage("Phiếu xuất phải chọn khách hàng.");
    }
}

// === Handler ===
public class UpdateOrderHandler(AppDbContext db) : IRequestHandler<UpdateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateOrderCommand request, CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == request.Id, ct);

        if (order is null)
            return Result<Guid>.Fail("Đơn hàng không tồn tại.");

        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
            return Result<Guid>.Fail("Không thể sửa đơn hàng đã hoàn thành hoặc đã hủy.");

        var code = string.IsNullOrWhiteSpace(request.Code) ? order.Code : request.Code;
        if (code != order.Code && await db.Orders.AnyAsync(o => o.Code == code, ct))
            return Result<Guid>.Fail($"Mã đơn hàng '{code}' đã tồn tại.");

        // Hoàn trả tồn kho từ chi tiết cũ
        foreach (var oldDetail in order.OrderDetails)
        {
            var providerIdToUse = oldDetail.ProviderId ?? order.ProviderId;
            var wareHouse = await db.WareHouses
                .FirstOrDefaultAsync(w => w.ProductId == oldDetail.ProductId && w.ProviderId == providerIdToUse, ct);

            if (wareHouse != null)
            {
                if (order.Type == OrderType.Import)
                {
                    wareHouse.Quantity -= oldDetail.Quantity;
                    wareHouse.TotalImport -= oldDetail.Quantity;
                }
                else
                {
                    wareHouse.Quantity += oldDetail.Quantity;
                    wareHouse.TotalExport -= oldDetail.Quantity;
                }
                wareHouse.LastStockUpdate = DateTime.UtcNow;
            }
        }

        // Kiểm tra tồn kho mới cho phiếu xuất
        if (request.Type == OrderType.Export)
        {
            foreach (var detail in request.Details)
            {
                var wareHouse = await db.WareHouses
                    .FirstOrDefaultAsync(w => w.ProductId == detail.ProductId, ct);

                if (wareHouse is null)
                    return Result<Guid>.Fail($"Sản phẩm {detail.ProductId} chưa có trong kho.");

                if (wareHouse.Quantity < detail.Quantity)
                {
                    var product = await db.Products.FindAsync([detail.ProductId], ct);
                    return Result<Guid>.Fail($"Sản phẩm '{product?.Name}' không đủ tồn kho. Hiện có: {wareHouse.Quantity}, yêu cầu: {detail.Quantity}.");
                }
            }
        }

        // Cập nhật thông tin chung
        order.Code = code;
        order.OrderDate = request.OrderDate;
        order.ProviderId = request.ProviderId;
        order.CustomerId = request.CustomerId;
        order.Discount = request.Discount;
        order.ShippingFee = request.ShippingFee;
        order.Note = request.Note;

        // Xóa chi tiết cũ và tạo chi tiết mới
        db.OrderDetails.RemoveRange(order.OrderDetails);
        order.OrderDetails.Clear();

        decimal totalAmount = 0;
        foreach (var detail in request.Details)
        {
            var lineTotal = detail.Quantity * detail.UnitPrice - detail.Discount + detail.Tax;
            totalAmount += lineTotal;

            order.OrderDetails.Add(new OrderDetail
            {
                ProductId = detail.ProductId,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                Discount = detail.Discount,
                Tax = detail.Tax,
                TotalPrice = lineTotal,
                ProviderId = detail.ProviderId
            });
        }

        order.TotalAmount = totalAmount;
        order.FinalAmount = totalAmount - request.Discount + request.ShippingFee;

        // Cập nhật lại tồn kho theo chi tiết mới
        foreach (var detail in request.Details)
        {
            var providerIdToUse = detail.ProviderId ?? request.ProviderId;
            var wareHouse = await db.WareHouses
                .FirstOrDefaultAsync(w => w.ProductId == detail.ProductId && w.ProviderId == providerIdToUse, ct);

            if (wareHouse == null)
            {
                wareHouse = new WareHouse
                {
                    ProductId = detail.ProductId,
                    ProviderId = providerIdToUse,
                    Quantity = 0
                };
                db.WareHouses.Add(wareHouse);
            }

            if (request.Type == OrderType.Import)
            {
                wareHouse.Quantity += detail.Quantity;
                wareHouse.TotalImport += detail.Quantity;
                wareHouse.CostPrice = detail.UnitPrice;
                wareHouse.ImportDate = request.OrderDate;
            }
            else
            {
                wareHouse.Quantity -= detail.Quantity;
                wareHouse.TotalExport += detail.Quantity;
            }

            wareHouse.LastStockUpdate = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(ct);

        return Result<Guid>.Ok(order.Id, "Cập nhật đơn hàng thành công.");
    }
}
