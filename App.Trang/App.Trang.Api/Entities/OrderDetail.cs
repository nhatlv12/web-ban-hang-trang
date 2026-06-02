using System.ComponentModel.DataAnnotations.Schema;

namespace App.Trang.Api.Entities;

/// <summary>
/// Chi tiết đơn hàng
/// </summary>
public class OrderDetail : BaseEntity
{
    /// <summary>Mã đơn hàng (FK)</summary>
    public Guid OrderId { get; set; }

    /// <summary>Mã sản phẩm (FK)</summary>
    public Guid ProductId { get; set; }

    /// <summary>Số lượng</summary>
    public int Quantity { get; set; }

    /// <summary>Đơn giá tại thời điểm giao dịch</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    /// <summary>Giảm giá trên dòng</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; } = 0;

    /// <summary>Thành tiền (Quantity × UnitPrice - Discount)</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    // Navigation properties
    /// <summary>Đơn hàng</summary>
    public Order Order { get; set; } = null!;

    /// <summary>Sản phẩm</summary>
    public Product Product { get; set; } = null!;
}
