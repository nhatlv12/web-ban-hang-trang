using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;

namespace App.Trang.Api.Features.Categories.Commands;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;

public class DeleteCategoryHandler(AppDbContext db) : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var entity = await db.Categories.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Fail("Không tìm thấy danh mục.");

        entity.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result.Ok("Xóa danh mục thành công.");
    }
}
