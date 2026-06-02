using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Providers.Queries;

// === Query ===
public record GetProviderByIdQuery(Guid Id) : IRequest<Result<ProviderDto>>;

// === Handler ===
public class GetProviderByIdHandler(AppDbContext db) : IRequestHandler<GetProviderByIdQuery, Result<ProviderDto>>
{
    public async Task<Result<ProviderDto>> Handle(GetProviderByIdQuery request, CancellationToken ct)
    {
        var provider = await db.Providers
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new ProviderDto(
                p.Id, p.Code, p.Name, p.Phone, p.Email, p.Address,
                p.TaxCode, p.ContactPerson, p.Note, p.IsActive,
                p.CreatedAt, p.UpdatedAt
            ))
            .FirstOrDefaultAsync(ct);

        return provider is null
            ? Result<ProviderDto>.Fail("Không tìm thấy nhà cung cấp.")
            : Result<ProviderDto>.Ok(provider);
    }
}
