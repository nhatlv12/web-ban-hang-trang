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
    decimal Discount,
    decimal Tax,
    Guid? ProviderId
);

// === Command ===
public record CreateOrderCommand(
    string Code,
    OrderType Type,
    DateTime OrderDate,
    Guid? ProviderId,
    Guid? CustomerId,
    decimal Discount,
    decimal ShippingFee,
    string? Note,
    string? CreatedBy,
    List<CreateOrderDetailDto> Details
) : IRequest<Result<Guid>>;

// === Validator ===
public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
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

        // Bỏ validate ProviderId ở header theo yêu cầu của user

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
        // Tự sinh mã nếu không truyền
        var code = string.IsNullOrWhiteSpace(request.Code)
            ? Common.Helpers.CodeGenerator.Generate(c => db.Orders.Any(o => o.Code == c), "ORD_")
            : request.Code;

        // Kiểm tra mã đơn hàng
        if (await db.Orders.AnyAsync(o => o.Code == code, ct))
            return Result<Guid>.Fail($"Mã đơn hàng '{code}' đã tồn tại.");

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
            Code = code,
            Type = request.Type,
            Status = request.Type == OrderType.Import ? OrderStatus.Completed : OrderStatus.Pending,
            OrderDate = request.OrderDate,
            ProviderId = request.ProviderId,
            CustomerId = request.CustomerId,
            Discount = request.Discount,
            ShippingFee = request.ShippingFee,
            Note = request.Note,
            CreatedBy = request.CreatedBy
        };

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

        db.Orders.Add(order);

        // Cập nhật tồn kho
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

        return Result<Guid>.Ok(order.Id, "Tạo đơn hàng thành công.");
    }
}
