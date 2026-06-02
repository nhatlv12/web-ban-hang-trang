using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Customers.Queries;

public record CustomerDto(
    Guid Id, string Code, string FullName, string Phone,
    string? Email, string? Address, DateTime? DateOfBirth,
    Gender? Gender, string? Note, bool IsActive,
    DateTime CreatedAt, DateTime? UpdatedAt
);

public record GetCustomersQuery() : IRequest<Result<List<CustomerDto>>>;

public class GetCustomersHandler(AppDbContext db) : IRequestHandler<GetCustomersQuery, Result<List<CustomerDto>>>
{
    public async Task<Result<List<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken ct)
    {
        var customers = await db.Customers
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CustomerDto(
                c.Id, c.Code, c.FullName, c.Phone, c.Email, c.Address,
                c.DateOfBirth, c.Gender, c.Note, c.IsActive, c.CreatedAt, c.UpdatedAt
            ))
            .ToListAsync(ct);

        return Result<List<CustomerDto>>.Ok(customers);
    }
}
