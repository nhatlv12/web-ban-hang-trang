using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Products.Commands;

public record UpdateProductCommand(
    Guid Id, string Code, string Name, string? Description,
    Guid CategoryId, Guid ProviderId,
    decimal CostPrice, decimal SellingPrice, decimal? OriginalPrice,
    string Unit, string? Image, bool IsNew, bool IsSale, bool IsActive
) : IRequest<Result>;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.CostPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SellingPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Image).MaximumLength(500);
    }
}

public class UpdateProductHandler(AppDbContext db) : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var entity = await db.Products.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Fail("Không tìm thấy sản phẩm.");

        if (await db.Products.AnyAsync(p => p.Code == request.Code && p.Id != request.Id, ct))
            return Result.Fail($"Mã sản phẩm '{request.Code}' đã tồn tại.");

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.CategoryId = request.CategoryId;
        entity.ProviderId = request.ProviderId;
        entity.CostPrice = request.CostPrice;
        entity.SellingPrice = request.SellingPrice;
        entity.OriginalPrice = request.OriginalPrice;
        entity.Unit = request.Unit;
        entity.Image = request.Image;
        entity.IsNew = request.IsNew;
        entity.IsSale = request.IsSale;
        entity.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);
        return Result.Ok("Cập nhật sản phẩm thành công.");
    }
}
