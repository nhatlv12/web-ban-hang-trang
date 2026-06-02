using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;

namespace App.Trang.Api.Features.Providers.Commands;

// === Command ===
public record DeleteProviderCommand(Guid Id) : IRequest<Result>;

// === Handler ===
public class DeleteProviderHandler(AppDbContext db) : IRequestHandler<DeleteProviderCommand, Result>
{
    public async Task<Result> Handle(DeleteProviderCommand request, CancellationToken ct)
    {
        var entity = await db.Providers.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Fail("Không tìm thấy nhà cung cấp.");

        // Soft delete
        entity.IsActive = false;
        await db.SaveChangesAsync(ct);

        return Result.Ok("Xóa nhà cung cấp thành công.");
    }
}
