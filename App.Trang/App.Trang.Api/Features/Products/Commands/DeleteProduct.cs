using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;

namespace App.Trang.Api.Features.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<Result>;

public class DeleteProductHandler(AppDbContext db) : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var entity = await db.Products.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Fail("Không tìm thấy sản phẩm.");

        entity.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result.Ok("Xóa sản phẩm thành công.");
    }
}
