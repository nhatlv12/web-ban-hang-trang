using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Orders.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;

public class GetOrderByIdHandler(AppDbContext db) : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Provider)
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
            .Include(o => o.OrderDetails).ThenInclude(d => d.Provider)
            .Where(o => o.Id == request.Id)
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
            .FirstOrDefaultAsync(ct);

        return order is null
            ? Result<OrderDto>.Fail("Không tìm thấy đơn hàng.")
            : Result<OrderDto>.Ok(order);
    }
}
