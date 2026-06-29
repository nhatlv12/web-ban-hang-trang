using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Trang.Api.Features.Products.Commands;

public class CreateProductCommand : IRequest<Result<Guid>>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal? OriginalPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
    public bool IsNew { get; set; }
    public bool IsSale { get; set; }
}

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Code).MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.CostPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SellingPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(50);
    }
}

public class CreateProductHandler(AppDbContext db, IWebHostEnvironment env) : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var code = string.IsNullOrWhiteSpace(request.Code)
            ? Common.Helpers.CodeGenerator.Generate(c => db.Products.Any(p => p.Code == c), "PRO_")
            : request.Code;

        if (db.Products.Any(p => p.Code == code))
            return Result<Guid>.Fail($"Mã sản phẩm '{code}' đã tồn tại.");

        if (!db.Categories.Any(c => c.Id == request.CategoryId))
            return Result<Guid>.Fail("Danh mục không tồn tại.");

        string? imagePath = null;
        if (request.Image is not null && request.Image.Length > 0)
        {
            var uploadsDir = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "uploads", "products");
            Directory.CreateDirectory(uploadsDir);
            var ext = Path.GetExtension(request.Image.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await request.Image.CopyToAsync(stream, ct);
            imagePath = $"/uploads/products/{fileName}";
        }

        var entity = new Product
        {
            Code = code,
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            CostPrice = request.CostPrice,
            SellingPrice = request.SellingPrice,
            OriginalPrice = request.OriginalPrice,
            Unit = request.Unit,
            Image = imagePath,
            IsNew = request.IsNew,
            IsSale = request.IsSale
        };

        db.Products.Add(entity);

        await db.SaveChangesAsync(ct);

        return Result<Guid>.Ok(entity.Id, "Tạo sản phẩm thành công.");
    }
}
