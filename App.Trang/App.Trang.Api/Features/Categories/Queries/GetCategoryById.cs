using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Categories.Queries;

public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDto>>;

public class GetCategoryByIdHandler(AppDbContext db) : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var category = await db.Categories
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoryDto(
                c.Id, c.Code, c.Name, c.Description, c.Icon,
                c.ParentId, c.SortOrder, c.IsActive,
                c.CreatedAt, c.UpdatedAt, null
            ))
            .FirstOrDefaultAsync(ct);

        return category is null
            ? Result<CategoryDto>.Fail("Không tìm thấy danh mục.")
            : Result<CategoryDto>.Ok(category);
    }
}
