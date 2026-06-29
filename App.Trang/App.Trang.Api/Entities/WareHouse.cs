using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Trang.Api.Entities;

/// <summary>
/// Kho hàng - quản lý tồn kho của từng sản phẩm
/// </summary>
public class WareHouse : BaseEntity
{
    /// <summary>Mã sản phẩm (FK, unique - quan hệ 1-1 với Product)</summary>
    public Guid ProductId { get; set; }

    /// <summary>Mã nhà cung cấp</summary>
    public Guid? ProviderId { get; set; }

    /// <summary>Số lượng tồn kho hiện tại</summary>
    public int Quantity { get; set; } = 0;

    /// <summary>Số lượng tồn kho tối thiểu (cảnh báo khi dưới mức này)</summary>
    public int MinQuantity { get; set; } = 0;

    /// <summary>Số lượng tồn kho tối đa (0 = không giới hạn)</summary>
    public int MaxQuantity { get; set; } = 0;

    /// <summary>Tổng số lượng đã nhập</summary>
    public int TotalImport { get; set; } = 0;

    /// <summary>Tổng số lượng đã xuất</summary>
    public int TotalExport { get; set; } = 0;

    /// <summary>Vị trí kho / kệ</summary>
    [MaxLength(200)]
    public string? Location { get; set; }

    /// <summary>Giá mua (giá nhập gần nhất)</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal CostPrice { get; set; } = 0;

    /// <summary>Ngày nhập kho gần nhất</summary>
    public DateTime? ImportDate { get; set; }

    /// <summary>Lần cập nhật tồn kho gần nhất</summary>
    public DateTime LastStockUpdate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>Sản phẩm tương ứng</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Nhà cung cấp</summary>
    public Provider? Provider { get; set; }
}
