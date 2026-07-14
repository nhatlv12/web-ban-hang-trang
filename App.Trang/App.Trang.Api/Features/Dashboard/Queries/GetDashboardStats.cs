using App.Trang.Api.Data;
using App.Trang.Api.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Features.Dashboard.Queries;

public record GetDashboardStatsQuery(string? Period) : IRequest<DashboardStatsDto>;

public class DashboardStatsDto
{
    // KPIs
    public int TotalOrders { get; set; }
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    
    // Trends (Percentage changes)
    public double OrdersTrend { get; set; }
    public double ProductsTrend { get; set; }
    public double CustomersTrend { get; set; }
    public double RevenueTrend { get; set; }
    
    // Revenue Line Chart (Doanh thu & Chi phí 12 tháng năm nay)
    public List<decimal> RevenueByMonth { get; set; } = new();
    public List<decimal> CostByMonth { get; set; } = new();
    
    // Import/Export Bar Chart (Nhập / Xuất kho 6 tháng gần nhất)
    public List<string> ImportExportLabels { get; set; } = new();
    public List<int> ImportCounts { get; set; } = new();
    public List<int> ExportCounts { get; set; } = new();
    
    // Category Doughnut Chart
    public List<string> CategoryLabels { get; set; } = new();
    public List<int> CategoryCounts { get; set; } = new();
    
    // Order Status Doughnut Chart
    public List<string> OrderStatusLabels { get; set; } = new();
    public List<int> OrderStatusCounts { get; set; } = new();
    
    // Quick Summary
    public decimal TotalImportMonth { get; set; }
    public decimal TotalExportMonth { get; set; }
    public decimal EstimatedProfit { get; set; }
    public int NewOrdersToday { get; set; }
    
    // Recent Orders
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
}

public class RecentOrderDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Type { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Status { get; set; }
    public string Date { get; set; } = string.Empty;
}

public class GetDashboardStatsHandler(AppDbContext db) : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var currentYear = now.Year;
        var currentMonth = now.Month;
        var startOfMonth = new DateTime(currentYear, currentMonth, 1);
        var startOfToday = now.Date;
        var startDate = DateTime.MinValue;
        var endDate = DateTime.MaxValue;

        switch (request.Period?.ToLower())
        {
            case "today":
                startDate = now.Date;
                endDate = startDate.AddDays(1);
                break;
            case "7d":
                startDate = now.Date.AddDays(-7);
                break;
            case "30d":
                startDate = now.Date.AddDays(-30);
                break;
            case "year":
                startDate = new DateTime(now.Year, 1, 1);
                endDate = startDate.AddYears(1);
                break;
            case "month":
            default:
                startDate = new DateTime(now.Year, now.Month, 1);
                endDate = startDate.AddMonths(1);
                break;
        }
        
        var prevStartDate = DateTime.MinValue;
        var prevEndDate = startDate;

        switch (request.Period?.ToLower())
        {
            case "today":
                prevStartDate = startDate.AddDays(-1);
                break;
            case "7d":
                prevStartDate = startDate.AddDays(-7);
                break;
            case "30d":
                prevStartDate = startDate.AddDays(-30);
                break;
            case "year":
                prevStartDate = startDate.AddYears(-1);
                break;
            case "month":
            default:
                prevStartDate = startDate.AddMonths(-1);
                break;
        }
        
        var dto = new DashboardStatsDto();

        // 1. KPIs
        dto.TotalOrders = await db.Orders.CountAsync(o => o.Type == OrderType.Export && o.OrderDate >= startDate && o.OrderDate < endDate, cancellationToken);
        dto.TotalProducts = await db.Products.CountAsync(p => p.IsActive, cancellationToken);
        dto.TotalCustomers = await db.Customers.CountAsync(c => c.IsActive, cancellationToken);
        dto.TotalRevenue = await db.Orders
            .Where(o => o.Type == OrderType.Export && o.Status == OrderStatus.Completed && o.OrderDate >= startDate && o.OrderDate < endDate)
            .SumAsync(o => o.FinalAmount, cancellationToken);

        // Calculate trends
        var prevOrders = await db.Orders.CountAsync(o => o.Type == OrderType.Export && o.OrderDate >= prevStartDate && o.OrderDate < prevEndDate, cancellationToken);
        dto.OrdersTrend = prevOrders == 0 ? (dto.TotalOrders > 0 ? 100 : 0) : Math.Round(((double)dto.TotalOrders - prevOrders) / prevOrders * 100, 1);

        var prevRevenue = await db.Orders
            .Where(o => o.Type == OrderType.Export && o.Status == OrderStatus.Completed && o.OrderDate >= prevStartDate && o.OrderDate < prevEndDate)
            .SumAsync(o => o.FinalAmount, cancellationToken);
        dto.RevenueTrend = prevRevenue == 0 ? (dto.TotalRevenue > 0 ? 100 : 0) : Math.Round((double)(dto.TotalRevenue - prevRevenue) / (double)prevRevenue * 100, 1);

        var prevProducts = await db.Products.CountAsync(p => p.IsActive && p.CreatedAt < prevEndDate, cancellationToken);
        dto.ProductsTrend = prevProducts == 0 ? (dto.TotalProducts > 0 ? 100 : 0) : Math.Round(((double)dto.TotalProducts - prevProducts) / prevProducts * 100, 1);

        var prevCustomers = await db.Customers.CountAsync(c => c.IsActive && c.CreatedAt < prevEndDate, cancellationToken);
        dto.CustomersTrend = prevCustomers == 0 ? (dto.TotalCustomers > 0 ? 100 : 0) : Math.Round(((double)dto.TotalCustomers - prevCustomers) / prevCustomers * 100, 1);

        // 2. Revenue Line Chart (Doanh thu & Chi phí 12 tháng năm nay)
        var ordersThisYear = await db.Orders
            .Where(o => o.OrderDate.Year == currentYear && o.Status == OrderStatus.Completed)
            .Select(o => new { o.Type, Month = o.OrderDate.Month, o.FinalAmount })
            .ToListAsync(cancellationToken);
            
        dto.RevenueByMonth = Enumerable.Range(1, 12)
            .Select(m => ordersThisYear.Where(o => o.Type == OrderType.Export && o.Month == m).Sum(o => o.FinalAmount))
            .ToList();
            
        dto.CostByMonth = Enumerable.Range(1, 12)
            .Select(m => ordersThisYear.Where(o => o.Type == OrderType.Import && o.Month == m).Sum(o => o.FinalAmount))
            .ToList();

        // 3. Import/Export Bar Chart (6 tháng gần nhất)
        var sixMonthsAgo = startOfMonth.AddMonths(-5);
        var ordersLast6Months = await db.Orders
            .Where(o => o.OrderDate >= sixMonthsAgo)
            .Select(o => new { o.Type, Year = o.OrderDate.Year, Month = o.OrderDate.Month })
            .ToListAsync(cancellationToken);

        for (int i = 5; i >= 0; i--)
        {
            var targetMonth = startOfMonth.AddMonths(-i);
            dto.ImportExportLabels.Add($"T{targetMonth.Month}");
            dto.ImportCounts.Add(ordersLast6Months.Count(o => o.Type == OrderType.Import && o.Year == targetMonth.Year && o.Month == targetMonth.Month));
            dto.ExportCounts.Add(ordersLast6Months.Count(o => o.Type == OrderType.Export && o.Year == targetMonth.Year && o.Month == targetMonth.Month));
        }

        // 4. Category Doughnut Chart
        var categories = await db.Categories
            .Select(c => new { c.Name, Count = c.Products.Count(p => p.IsActive) })
            .OrderByDescending(c => c.Count)
            .Take(5)
            .ToListAsync(cancellationToken);
            
        dto.CategoryLabels = categories.Select(c => c.Name).ToList();
        dto.CategoryCounts = categories.Select(c => c.Count).ToList();

        // 5. Order Status Doughnut Chart (for export orders or all orders?)
        var statusGroups = await db.Orders
            .Where(o => o.OrderDate >= startDate && o.OrderDate < endDate)
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        // Status Map: 0=Draft, 1=Pending, 2=Confirmed, 3=Completed, 4=Cancelled
        var allStatuses = new[] { OrderStatus.Draft, OrderStatus.Pending, OrderStatus.Confirmed, OrderStatus.Completed, OrderStatus.Cancelled };
        var statusNames = new[] { "Nháp", "Chờ xử lý", "Đã xác nhận", "Hoàn thành", "Đã hủy" };
        
        foreach (var st in allStatuses)
        {
            dto.OrderStatusLabels.Add(statusNames[(int)st]);
            dto.OrderStatusCounts.Add(statusGroups.FirstOrDefault(g => g.Status == st)?.Count ?? 0);
        }

        // 6. Quick Summary
        dto.TotalImportMonth = await db.Orders
            .Where(o => o.Type == OrderType.Import && o.Status == OrderStatus.Completed && o.OrderDate >= startDate && o.OrderDate < endDate)
            .SumAsync(o => o.FinalAmount, cancellationToken);
            
        dto.TotalExportMonth = await db.Orders
            .Where(o => o.Type == OrderType.Export && o.Status == OrderStatus.Completed && o.OrderDate >= startDate && o.OrderDate < endDate)
            .SumAsync(o => o.FinalAmount, cancellationToken);
            
        dto.EstimatedProfit = dto.TotalExportMonth - dto.TotalImportMonth;
        
        dto.NewOrdersToday = await db.Orders
            .CountAsync(o => o.Type == OrderType.Export && o.OrderDate >= startOfToday, cancellationToken);

        // 7. Recent Orders
        dto.RecentOrders = await db.Orders
            .OrderByDescending(o => o.OrderDate)
            .Take(5)
            .Select(o => new RecentOrderDto
            {
                Id = o.Id,
                Code = o.Code,
                Type = (int)o.Type,
                Customer = o.Customer != null ? o.Customer.FullName : string.Empty,
                Provider = o.Provider != null ? o.Provider.Name : string.Empty,
                Amount = o.FinalAmount,
                Status = (int)o.Status,
                Date = o.OrderDate.ToString("dd/MM/yyyy HH:mm")
            })
            .ToListAsync(cancellationToken);

        return dto;
    }
}
