namespace App.Trang.Api.Entities;

/// <summary>
/// Loại đơn hàng: Nhập hoặc Xuất
/// </summary>
public enum OrderType
{
    /// <summary>Nhập hàng (từ nhà cung cấp)</summary>
    Import = 1,

    /// <summary>Xuất hàng (bán cho khách hàng)</summary>
    Export = 2
}

/// <summary>
/// Trạng thái đơn hàng
/// </summary>
public enum OrderStatus
{
    /// <summary>Nháp</summary>
    Draft = 0,

    /// <summary>Chờ xử lý</summary>
    Pending = 1,

    /// <summary>Đã xác nhận</summary>
    Confirmed = 2,

    /// <summary>Hoàn thành</summary>
    Completed = 3,

    /// <summary>Đã hủy</summary>
    Cancelled = 4
}

/// <summary>
/// Giới tính
/// </summary>
public enum Gender
{
    /// <summary>Nam</summary>
    Male = 1,

    /// <summary>Nữ</summary>
    Female = 2,

    /// <summary>Khác</summary>
    Other = 3
}
