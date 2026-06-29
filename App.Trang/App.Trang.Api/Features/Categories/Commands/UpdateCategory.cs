using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Categories.Commands;

public record UpdateCategoryCommand(
    Guid Id, string Code, string Name, string? Description,
    string? Icon, Guid? ParentId, int SortOrder, bool IsActive
) : IRequest<Result>;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Icon).MaximumLength(50);
    }
}

public class UpdateCategoryHandler(AppDbContext db) : IRequestHandler<UpdateCategoryCommand, Result>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var entity = await db.Categories.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Fail("Không tìm thấy danh mục.");

        if (await db.Categories.AnyAsync(c => c.Code == request.Code && c.Id != request.Id, ct))
            return Result.Fail($"Mã danh mục '{request.Code}' đã tồn tại.");

        if (request.ParentId.HasValue && request.ParentId == request.Id)
            return Result.Fail("Danh mục không thể là cha của chính nó.");

        if (!string.IsNullOrEmpty(request.Code))
            entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Icon = request.Icon;
        entity.ParentId = request.ParentId;
        entity.SortOrder = request.SortOrder;
        entity.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);
        return Result.Ok("Cập nhật danh mục thành công.");
    }
}
