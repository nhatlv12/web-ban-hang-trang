using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;

namespace App.Trang.Api.Features.Customers.Commands;

public record DeleteCustomerCommand(Guid Id) : IRequest<Result>;

public class DeleteCustomerHandler(AppDbContext db) : IRequestHandler<DeleteCustomerCommand, Result>
{
    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var entity = await db.Customers.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Fail("Không tìm thấy khách hàng.");

        entity.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result.Ok("Xóa khách hàng thành công.");
    }
}
