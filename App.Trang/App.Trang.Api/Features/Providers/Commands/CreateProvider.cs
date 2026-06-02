using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using FluentValidation;
using MediatR;

namespace App.Trang.Api.Features.Providers.Commands;

// === Command ===
public record CreateProviderCommand(
    string Code,
    string Name,
    string? Phone,
    string? Email,
    string? Address,
    string? TaxCode,
    string? ContactPerson,
    string? Note
) : IRequest<Result<Guid>>;

// === Validator ===
public class CreateProviderValidator : AbstractValidator<CreateProviderCommand>
{
    public CreateProviderValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
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
public class CreateProviderHandler(AppDbContext db) : IRequestHandler<CreateProviderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProviderCommand request, CancellationToken ct)
    {
        // Kiểm tra Code đã tồn tại
        if (db.Providers.Any(p => p.Code == request.Code))
            return Result<Guid>.Fail($"Mã nhà cung cấp '{request.Code}' đã tồn tại.");

        var entity = new Provider
        {
            Code = request.Code,
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            TaxCode = request.TaxCode,
            ContactPerson = request.ContactPerson,
            Note = request.Note
        };

        db.Providers.Add(entity);
        await db.SaveChangesAsync(ct);

        return Result<Guid>.Ok(entity.Id, "Tạo nhà cung cấp thành công.");
    }
}
