using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using FluentValidation;
using MediatR;

namespace App.Trang.Api.Features.Categories.Commands;

public record CreateCategoryCommand(
    string Code, string Name, string? Description,
    string? Icon, Guid? ParentId, int SortOrder
) : IRequest<Result<Guid>>;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Icon).MaximumLength(50);
    }
}

public class CreateCategoryHandler(AppDbContext db) : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        if (db.Categories.Any(c => c.Code == request.Code))
            return Result<Guid>.Fail($"Mã danh mục '{request.Code}' đã tồn tại.");

        if (request.ParentId.HasValue && !db.Categories.Any(c => c.Id == request.ParentId.Value))
            return Result<Guid>.Fail("Danh mục cha không tồn tại.");

        var entity = new Category
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            Icon = request.Icon,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder
        };

        db.Categories.Add(entity);
        await db.SaveChangesAsync(ct);

        return Result<Guid>.Ok(entity.Id, "Tạo danh mục thành công.");
    }
}
