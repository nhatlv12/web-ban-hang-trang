using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Orders.Commands;

// === DTO cho chi tiết đơn hàng khi tạo ===
public record CreateOrderDetailDto(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal Discount
);

// === Command ===
public record CreateOrderCommand(
    string Code,
    OrderType Type,
    DateTime OrderDate,
    Guid? ProviderId,
    Guid? CustomerId,
    decimal Discount,
    string? Note,
    string? CreatedBy,
    List<CreateOrderDetailDto> Details
) : IRequest<Result<Guid>>;

// === Validator ===
public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
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

        // Phiếu nhập phải có nhà cung cấp
        RuleFor(x => x.ProviderId)
            .NotEmpty().When(x => x.Type == OrderType.Import)
            .WithMessage("Phiếu nhập phải chọn nhà cung cấp.");

        // Phiếu xuất phải có khách hàng
        RuleFor(x => x.CustomerId)
            .NotEmpty().When(x => x.Type == OrderType.Export)
            .WithMessage("Phiếu xuất phải chọn khách hàng.");
    }
}

// === Handler ===
public class CreateOrderHandler(AppDbContext db) : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        // Kiểm tra mã đơn hàng
        if (await db.Orders.AnyAsync(o => o.Code == request.Code, ct))
            return Result<Guid>.Fail($"Mã đơn hàng '{request.Code}' đã tồn tại.");

        // Kiểm tra tồn kho khi xuất hàng
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
                    return Result<Guid>.Fail(
                        $"Sản phẩm '{product?.Name}' không đủ tồn kho. Hiện có: {wareHouse.Quantity}, yêu cầu: {detail.Quantity}.");
                }
            }
        }

        // Tạo đơn hàng
        var order = new Order
        {
            Code = request.Code,
            Type = request.Type,
            Status = OrderStatus.Pending,
            OrderDate = request.OrderDate,
            ProviderId = request.ProviderId,
            CustomerId = request.CustomerId,
            Discount = request.Discount,
            Note = request.Note,
            CreatedBy = request.CreatedBy
        };

        decimal totalAmount = 0;
        foreach (var detail in request.Details)
        {
            var lineTotal = detail.Quantity * detail.UnitPrice - detail.Discount;
            totalAmount += lineTotal;

            order.OrderDetails.Add(new OrderDetail
            {
                ProductId = detail.ProductId,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                Discount = detail.Discount,
                TotalPrice = lineTotal
            });
        }

        order.TotalAmount = totalAmount;
        order.FinalAmount = totalAmount - request.Discount;

        db.Orders.Add(order);

        // Cập nhật tồn kho
        foreach (var detail in request.Details)
        {
            var wareHouse = await db.WareHouses
                .FirstOrDefaultAsync(w => w.ProductId == detail.ProductId, ct);

            if (wareHouse != null)
            {
                if (request.Type == OrderType.Import)
                    wareHouse.Quantity += detail.Quantity;
                else
                    wareHouse.Quantity -= detail.Quantity;

                wareHouse.LastStockUpdate = DateTime.UtcNow;
            }
        }

        await db.SaveChangesAsync(ct);

        return Result<Guid>.Ok(order.Id, "Tạo đơn hàng thành công.");
    }
}
