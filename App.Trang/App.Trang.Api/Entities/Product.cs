using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Trang.Api.Entities;

/// <summary>
/// Sản phẩm
/// </summary>
public class Product : BaseEntity
{
    /// <summary>Mã sản phẩm (VD: SP001)</summary>
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Tên sản phẩm</summary>
    [Required, MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Mô tả chi tiết</summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>Mã danh mục (FK)</summary>
    public Guid CategoryId { get; set; }

    /// <summary>Giá nhập (giá vốn)</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal CostPrice { get; set; }

    /// <summary>Giá bán</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal SellingPrice { get; set; }

    /// <summary>Giá gốc (trước khuyến mãi)</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? OriginalPrice { get; set; }

    /// <summary>Đơn vị tính (cái, hộp, kg...)</summary>
    [Required, MaxLength(50)]
    public string Unit { get; set; } = string.Empty;

    /// <summary>URL hình ảnh chính</summary>
    [MaxLength(500)]
    public string? Image { get; set; }

    /// <summary>Sản phẩm mới</summary>
    public bool IsNew { get; set; } = false;

    /// <summary>Đang khuyến mãi</summary>
    public bool IsSale { get; set; } = false;

    /// <summary>Trạng thái hoạt động</summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    /// <summary>Danh mục sản phẩm</summary>
    public Category Category { get; set; } = null!;

    /// <summary>Thông tin kho hàng</summary>
    public WareHouse? WareHouse { get; set; }

    /// <summary>Danh sách chi tiết đơn hàng liên quan</summary>
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
