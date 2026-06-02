using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Categories.Queries;

public record CategoryDto(
    Guid Id, string Code, string Name, string? Description,
    string? Icon, Guid? ParentId, int SortOrder, bool IsActive,
    DateTime CreatedAt, DateTime? UpdatedAt,
    List<CategoryDto>? Children
);

public record GetCategoriesQuery() : IRequest<Result<List<CategoryDto>>>;

public class GetCategoriesHandler(AppDbContext db) : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var allCategories = await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);

        // Xây dựng cây danh mục
        var rootCategories = allCategories
            .Where(c => c.ParentId == null)
            .Select(c => MapToDto(c, allCategories))
            .ToList();

        return Result<List<CategoryDto>>.Ok(rootCategories);
    }

    private static CategoryDto MapToDto(Entities.Category category, List<Entities.Category> allCategories)
    {
        var children = allCategories
            .Where(c => c.ParentId == category.Id)
            .Select(c => MapToDto(c, allCategories))
            .ToList();

        return new CategoryDto(
            category.Id, category.Code, category.Name, category.Description,
            category.Icon, category.ParentId, category.SortOrder, category.IsActive,
            category.CreatedAt, category.UpdatedAt,
            children.Count > 0 ? children : null
        );
    }
}
