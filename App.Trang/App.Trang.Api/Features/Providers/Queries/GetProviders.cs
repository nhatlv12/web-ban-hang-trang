using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Providers.Queries;

// === DTO ===
public record ProviderDto(
    Guid Id,
    string Code,
    string Name,
    string? Phone,
    string? Email,
    string? Address,
    string? TaxCode,
    string? ContactPerson,
    string? Note,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

// === Query ===
public record GetProvidersQuery() : IRequest<Result<List<ProviderDto>>>;

// === Handler ===
public class GetProvidersHandler(AppDbContext db) : IRequestHandler<GetProvidersQuery, Result<List<ProviderDto>>>
{
    public async Task<Result<List<ProviderDto>>> Handle(GetProvidersQuery request, CancellationToken ct)
    {
        var providers = await db.Providers
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProviderDto(
                p.Id, p.Code, p.Name, p.Phone, p.Email, p.Address,
                p.TaxCode, p.ContactPerson, p.Note, p.IsActive,
                p.CreatedAt, p.UpdatedAt
            ))
            .ToListAsync(ct);

        return Result<List<ProviderDto>>.Ok(providers);
    }
}
