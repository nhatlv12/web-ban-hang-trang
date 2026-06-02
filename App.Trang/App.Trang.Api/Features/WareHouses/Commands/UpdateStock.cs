using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using FluentValidation;
using MediatR;

namespace App.Trang.Api.Features.WareHouses.Commands;

public record UpdateStockCommand(
    Guid Id,
    int Quantity,
    int MinQuantity,
    int MaxQuantity,
    string? Location
) : IRequest<Result>;

public class UpdateStockValidator : AbstractValidator<UpdateStockCommand>
{
    public UpdateStockValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MinQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Location).MaximumLength(200);
    }
}

public class UpdateStockHandler(AppDbContext db) : IRequestHandler<UpdateStockCommand, Result>
{
    public async Task<Result> Handle(UpdateStockCommand request, CancellationToken ct)
    {
        var entity = await db.WareHouses.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Fail("Không tìm thấy bản ghi kho.");

        entity.Quantity = request.Quantity;
        entity.MinQuantity = request.MinQuantity;
        entity.MaxQuantity = request.MaxQuantity;
        entity.Location = request.Location;
        entity.LastStockUpdate = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return Result.Ok("Cập nhật tồn kho thành công.");
    }
}
