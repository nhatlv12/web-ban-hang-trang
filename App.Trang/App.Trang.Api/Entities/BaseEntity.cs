namespace App.Trang.Api.Entities;

/// <summary>
/// Entity cơ sở chứa các trường chung cho tất cả entities
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Khóa chính</summary>
    public Guid Id { get; set; }

    /// <summary>Ngày tạo</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Ngày cập nhật</summary>
    public DateTime? UpdatedAt { get; set; }
}
