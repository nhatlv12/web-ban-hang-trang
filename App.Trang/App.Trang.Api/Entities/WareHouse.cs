using System.ComponentModel.DataAnnotations;

namespace App.Trang.Api.Entities;

/// <summary>
/// Kho hàng - quản lý tồn kho của từng sản phẩm
/// </summary>
public class WareHouse : BaseEntity
{
    /// <summary>Mã sản phẩm (FK, unique - quan hệ 1-1 với Product)</summary>
    public Guid ProductId { get; set; }

    /// <summary>Số lượng tồn kho hiện tại</summary>
    public int Quantity { get; set; } = 0;

    /// <summary>Số lượng tồn kho tối thiểu (cảnh báo khi dưới mức này)</summary>
    public int MinQuantity { get; set; } = 0;

    /// <summary>Số lượng tồn kho tối đa (0 = không giới hạn)</summary>
    public int MaxQuantity { get; set; } = 0;

    /// <summary>Vị trí kho / kệ</summary>
    [MaxLength(200)]
    public string? Location { get; set; }

    /// <summary>Lần cập nhật tồn kho gần nhất</summary>
    public DateTime LastStockUpdate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>Sản phẩm tương ứng</summary>
    public Product Product { get; set; } = null!;
}
