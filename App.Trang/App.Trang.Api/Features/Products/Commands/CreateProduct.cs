using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using FluentValidation;
using MediatR;

namespace App.Trang.Api.Features.Products.Commands;

public record CreateProductCommand(
    string Code, string Name, string? Description,
    Guid CategoryId, Guid ProviderId,
    decimal CostPrice, decimal SellingPrice, decimal? OriginalPrice,
    string Unit, string? Image, bool IsNew, bool IsSale
) : IRequest<Result<Guid>>;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
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

public class CreateProductHandler(AppDbContext db) : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        if (db.Products.Any(p => p.Code == request.Code))
            return Result<Guid>.Fail($"Mã sản phẩm '{request.Code}' đã tồn tại.");

        if (!db.Categories.Any(c => c.Id == request.CategoryId))
            return Result<Guid>.Fail("Danh mục không tồn tại.");

        if (!db.Providers.Any(p => p.Id == request.ProviderId))
            return Result<Guid>.Fail("Nhà cung cấp không tồn tại.");

        var entity = new Product
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            ProviderId = request.ProviderId,
            CostPrice = request.CostPrice,
            SellingPrice = request.SellingPrice,
            OriginalPrice = request.OriginalPrice,
            Unit = request.Unit,
            Image = request.Image,
            IsNew = request.IsNew,
            IsSale = request.IsSale
        };

        db.Products.Add(entity);

        // Tự động tạo bản ghi kho hàng
        var wareHouse = new WareHouse
        {
            ProductId = entity.Id,
            Quantity = 0
        };
        db.WareHouses.Add(wareHouse);

        await db.SaveChangesAsync(ct);

        return Result<Guid>.Ok(entity.Id, "Tạo sản phẩm thành công.");
    }
}
