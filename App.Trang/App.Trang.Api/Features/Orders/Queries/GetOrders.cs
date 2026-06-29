using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Orders.Queries;

public record OrderDto(
    Guid Id, string Code, OrderType Type, OrderStatus Status,
    DateTime OrderDate, Guid? ProviderId, string? ProviderName,
    Guid? CustomerId, string? CustomerName,
    decimal TotalAmount, decimal Discount, decimal ShippingFee, decimal FinalAmount,
    string? Note, string? CreatedBy,
    DateTime CreatedAt, DateTime? UpdatedAt,
    List<OrderDetailDto>? Details
);

public record OrderDetailDto(
    Guid Id, Guid ProductId, string ProductCode, string ProductName,
    Guid? ProviderId, string? ProviderName,
    int Quantity, decimal UnitPrice, decimal Discount, decimal Tax, decimal TotalPrice
);

public record GetOrdersQuery(OrderType? Type) : IRequest<Result<List<OrderDto>>>;

public class GetOrdersHandler(AppDbContext db) : IRequestHandler<GetOrdersQuery, Result<List<OrderDto>>>
{
    public async Task<Result<List<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        var query = db.Orders
            .AsNoTracking()
            .Include(o => o.Provider)
            .Include(o => o.Customer)
            .AsQueryable();

        if (request.Type.HasValue)
            query = query.Where(o => o.Type == request.Type.Value);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto(
                o.Id, o.Code, o.Type, o.Status, o.OrderDate,
                o.ProviderId, o.Provider != null ? o.Provider.Name : null,
                o.CustomerId, o.Customer != null ? o.Customer.FullName : null,
                o.TotalAmount, o.Discount, o.ShippingFee, o.FinalAmount,
                o.Note, o.CreatedBy, o.CreatedAt, o.UpdatedAt,
                o.OrderDetails.Select(d => new OrderDetailDto(
                    d.Id, d.ProductId, d.Product.Code, d.Product.Name,
                    d.ProviderId, d.Provider != null ? d.Provider.Name : null,
                    d.Quantity, d.UnitPrice, d.Discount, d.Tax, d.TotalPrice
                )).ToList()
            ))
            .ToListAsync(ct);

        return Result<List<OrderDto>>.Ok(orders);
    }
}
