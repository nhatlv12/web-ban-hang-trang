using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Trang.Api.Entities;

/// <summary>
/// Đơn hàng (phiếu nhập/xuất)
/// </summary>
public class Order : BaseEntity
{
    /// <summary>Mã đơn hàng (VD: PN001 cho nhập, PX001 cho xuất)</summary>
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Loại đơn hàng: Nhập (Import) hoặc Xuất (Export)</summary>
    public OrderType Type { get; set; }

    /// <summary>Trạng thái đơn hàng</summary>
    public OrderStatus Status { get; set; } = OrderStatus.Draft;

    /// <summary>Ngày lập phiếu</summary>
    public DateTime OrderDate { get; set; }

    /// <summary>Mã nhà cung cấp (FK) - dùng khi Type = Import</summary>
    public Guid? ProviderId { get; set; }

    /// <summary>Mã khách hàng (FK) - dùng khi Type = Export</summary>
    public Guid? CustomerId { get; set; }

    /// <summary>Tổng tiền</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    /// <summary>Giảm giá</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; } = 0;

    /// <summary>Thành tiền (TotalAmount - Discount)</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal FinalAmount { get; set; }

    /// <summary>Ghi chú</summary>
    [MaxLength(1000)]
    public string? Note { get; set; }

    /// <summary>Phí vận chuyển</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingFee { get; set; } = 0;

    /// <summary>Người tạo</summary>
    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    // Navigation properties
    /// <summary>Nhà cung cấp (khi nhập hàng)</summary>
    public Provider? Provider { get; set; }

    /// <summary>Khách hàng (khi xuất hàng)</summary>
    public Customer? Customer { get; set; }

    /// <summary>Danh sách chi tiết đơn hàng</summary>
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
