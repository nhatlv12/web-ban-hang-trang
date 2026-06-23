using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Products.Commands;

public class UpdateProductCommand : IRequest<Result>
{
    public Guid Id { get; set; }
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
    public bool IsActive { get; set; }
}

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.CostPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SellingPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(50);
    }
}

public class UpdateProductHandler(AppDbContext db, IWebHostEnvironment env) : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var entity = await db.Products.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Fail("Không tìm thấy sản phẩm.");

        if (await db.Products.AnyAsync(p => p.Code == request.Code && p.Id != request.Id, ct))
            return Result.Fail($"Mã sản phẩm '{request.Code}' đã tồn tại.");

        // Xử lý upload ảnh mới
        if (request.Image is not null && request.Image.Length > 0)
        {
            // Xóa ảnh cũ nếu có
            if (!string.IsNullOrEmpty(entity.Image))
            {
                var oldPath = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), entity.Image.TrimStart('/'));
                if (File.Exists(oldPath)) File.Delete(oldPath);
            }

            var uploadsDir = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "uploads", "products");
            Directory.CreateDirectory(uploadsDir);
            var ext = Path.GetExtension(request.Image.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await request.Image.CopyToAsync(stream, ct);
            entity.Image = $"/uploads/products/{fileName}";
        }

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.CategoryId = request.CategoryId;
        entity.CostPrice = request.CostPrice;
        entity.SellingPrice = request.SellingPrice;
        entity.OriginalPrice = request.OriginalPrice;
        entity.Unit = request.Unit;
        entity.IsNew = request.IsNew;
        entity.IsSale = request.IsSale;
        entity.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);
        return Result.Ok("Cập nhật sản phẩm thành công.");
    }
}
