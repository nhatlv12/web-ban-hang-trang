using App.Trang.Api.Common.Models;
using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Customers.Commands;

// === Command ===
public record UpdateCustomerCommand(
    Guid Id,
    string Code,
    string FullName,
    string Phone,
    string? Email,
    string? Address,
    DateTime? DateOfBirth,
    Gender? Gender,
    string? Note,
    bool IsActive
) : IRequest<Result>;

// === Validator ===
public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).MaximumLength(20);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(15);
        RuleFor(x => x.Email).MaximumLength(100).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.Note).MaximumLength(1000);
    }
}

// === Handler ===
public class UpdateCustomerHandler(AppDbContext db) : IRequestHandler<UpdateCustomerCommand, Result>
{
    public async Task<Result> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var entity = await db.Customers.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Fail("Không tìm thấy khách hàng.");

        if (await db.Customers.AnyAsync(c => c.Code == request.Code && c.Id != request.Id, ct))
            return Result.Fail($"Mã khách hàng '{request.Code}' đã tồn tại.");

        if (!string.IsNullOrEmpty(request.Code))
            entity.Code = request.Code;
        entity.FullName = request.FullName;
        entity.Phone = request.Phone;
        entity.Email = request.Email;
        entity.Address = request.Address;
        entity.DateOfBirth = request.DateOfBirth;
        entity.Gender = request.Gender;
        entity.Note = request.Note;
        entity.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);
        return Result.Ok("Cập nhật khách hàng thành công.");
    }
}
