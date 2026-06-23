using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using FluentValidation;
using MediatR;

namespace App.Trang.Api.Features.Customers.Commands;

// === Command ===
public record CreateCustomerCommand(
    string Code,
    string FullName,
    string Phone,
    string? Email,
    string? Address,
    DateTime? DateOfBirth,
    Gender? Gender,
    string? Note
) : IRequest<Result<Guid>>;

// === Validator ===
public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Code).MaximumLength(20);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(15);
        RuleFor(x => x.Email).MaximumLength(100).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.Note).MaximumLength(1000);
    }
}

// === Handler ===
public class CreateCustomerHandler(AppDbContext db) : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        var code = string.IsNullOrWhiteSpace(request.Code)
            ? Common.Helpers.CodeGenerator.Generate(c => db.Customers.Any(x => x.Code == c), "CUS_")
            : request.Code;

        if (db.Customers.Any(c => c.Code == code))
            return Result<Guid>.Fail($"Mã khách hàng '{code}' đã tồn tại.");

        var entity = new Customer
        {
            Code = code,
            FullName = request.FullName,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Note = request.Note
        };

        db.Customers.Add(entity);
        await db.SaveChangesAsync(ct);

        return Result<Guid>.Ok(entity.Id, "Tạo khách hàng thành công.");
    }
}
