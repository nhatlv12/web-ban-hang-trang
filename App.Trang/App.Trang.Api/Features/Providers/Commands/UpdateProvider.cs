using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Providers.Commands;

// === Command ===
public record UpdateProviderCommand(
    Guid Id,
    string Code,
    string Name,
    string? Phone,
    string? Email,
    string? Address,
    string? TaxCode,
    string? ContactPerson,
    string? Note,
    bool IsActive
) : IRequest<Result>;

// === Validator ===
public class UpdateProviderValidator : AbstractValidator<UpdateProviderCommand>
{
    public UpdateProviderValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(15);
        RuleFor(x => x.Email).MaximumLength(100).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.TaxCode).MaximumLength(20);
        RuleFor(x => x.ContactPerson).MaximumLength(100);
        RuleFor(x => x.Note).MaximumLength(1000);
    }
}

// === Handler ===
public class UpdateProviderHandler(AppDbContext db) : IRequestHandler<UpdateProviderCommand, Result>
{
    public async Task<Result> Handle(UpdateProviderCommand request, CancellationToken ct)
    {
        var entity = await db.Providers.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Fail("Không tìm thấy nhà cung cấp.");

        // Kiểm tra Code trùng (trừ chính nó)
        if (await db.Providers.AnyAsync(p => p.Code == request.Code && p.Id != request.Id, ct))
            return Result.Fail($"Mã nhà cung cấp '{request.Code}' đã tồn tại.");

        if (!string.IsNullOrEmpty(request.Code))
            entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Phone = request.Phone;
        entity.Email = request.Email;
        entity.Address = request.Address;
        entity.TaxCode = request.TaxCode;
        entity.ContactPerson = request.ContactPerson;
        entity.Note = request.Note;
        entity.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);
        return Result.Ok("Cập nhật nhà cung cấp thành công.");
    }
}
