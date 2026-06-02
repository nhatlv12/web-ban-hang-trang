using System.ComponentModel.DataAnnotations;

namespace App.Trang.Api.Entities;

/// <summary>
/// Khách hàng mua hàng
/// </summary>
public class Customer : BaseEntity
{
    /// <summary>Mã khách hàng (VD: KH001)</summary>
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Họ tên khách hàng</summary>
    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>Số điện thoại</summary>
    [Required, MaxLength(15)]
    public string Phone { get; set; } = string.Empty;

    /// <summary>Email</summary>
    [MaxLength(100)]
    public string? Email { get; set; }

    /// <summary>Địa chỉ giao hàng</summary>
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>Ngày sinh</summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>Giới tính</summary>
    public Gender? Gender { get; set; }

    /// <summary>Ghi chú</summary>
    [MaxLength(1000)]
    public string? Note { get; set; }

    /// <summary>Trạng thái hoạt động</summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    /// <summary>Danh sách đơn hàng xuất của khách hàng này</summary>
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
