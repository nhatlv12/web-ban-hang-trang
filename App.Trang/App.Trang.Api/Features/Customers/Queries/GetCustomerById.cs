using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Customers.Queries;

public record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;

public class GetCustomerByIdHandler(AppDbContext db) : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var customer = await db.Customers
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CustomerDto(
                c.Id, c.Code, c.FullName, c.Phone, c.Email, c.Address,
                c.DateOfBirth, c.Gender, c.Note, c.IsActive, c.CreatedAt, c.UpdatedAt
            ))
            .FirstOrDefaultAsync(ct);

        return customer is null
            ? Result<CustomerDto>.Fail("Không tìm thấy khách hàng.")
            : Result<CustomerDto>.Ok(customer);
    }
}
