using System.ComponentModel.DataAnnotations;

namespace App.Trang.Api.Entities;

/// <summary>
/// Nhà cung cấp hàng hóa
/// </summary>
public class Provider : BaseEntity
{
    /// <summary>Mã nhà cung cấp (VD: NCC001)</summary>
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Tên nhà cung cấp</summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Số điện thoại</summary>
    [MaxLength(15)]
    public string? Phone { get; set; }

    /// <summary>Email liên hệ</summary>
    [MaxLength(100)]
    public string? Email { get; set; }

    /// <summary>Địa chỉ</summary>
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>Mã số thuế</summary>
    [MaxLength(20)]
    public string? TaxCode { get; set; }

    /// <summary>Người liên hệ</summary>
    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    /// <summary>Ghi chú</summary>
    [MaxLength(1000)]
    public string? Note { get; set; }

    /// <summary>Trạng thái hoạt động</summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    /// <summary>Danh sách sản phẩm do nhà cung cấp này cung cấp</summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();

    /// <summary>Danh sách đơn hàng nhập từ nhà cung cấp này</summary>
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
