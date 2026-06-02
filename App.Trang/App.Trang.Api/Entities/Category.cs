using System.ComponentModel.DataAnnotations;

namespace App.Trang.Api.Entities;

/// <summary>
/// Danh mục sản phẩm (hỗ trợ đa cấp cha-con)
/// </summary>
public class Category : BaseEntity
{
    /// <summary>Mã danh mục (VD: DM001)</summary>
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Tên danh mục</summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Mô tả</summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>Icon (class name, VD: pi pi-mobile)</summary>
    [MaxLength(50)]
    public string? Icon { get; set; }

    /// <summary>Mã danh mục cha (FK, self-reference)</summary>
    public Guid? ParentId { get; set; }

    /// <summary>Thứ tự hiển thị</summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>Trạng thái hoạt động</summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    /// <summary>Danh mục cha</summary>
    public Category? Parent { get; set; }

    /// <summary>Danh sách danh mục con</summary>
    public ICollection<Category> Children { get; set; } = new List<Category>();

    /// <summary>Danh sách sản phẩm thuộc danh mục này</summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
