using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Business.DTOs
{
    // Sales Reports
    public class SalesReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalVehiclesSold { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public List<SalesTrendDto> DailySales { get; set; } = new List<SalesTrendDto>();
        public List<TopSellerDto> TopSellers { get; set; } = new List<TopSellerDto>();
        public List<TopVehicleDto> TopVehicles { get; set; } = new List<TopVehicleDto>();
    }

    public class SalesTrendDto
    {
        public DateTime Date { get; set; }
        public int Orders { get; set; }
        public decimal Revenue { get; set; }
        public int VehiclesSold { get; set; }
        public decimal Profit { get; set; }
    }

    public class TopSellerDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string DealerName { get; set; } = string.Empty;
        public int OrdersSold { get; set; }
        public decimal Revenue { get; set; }
        public decimal Commission { get; set; }
        public decimal AchievementRate { get; set; }
    }

    public class TopVehicleDto
    {
        public Guid VehicleModelId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public decimal Profit { get; set; }
        public decimal MarketShare { get; set; }
    }

    // Customer Reports
    public class CustomerReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalCustomers { get; set; }
        public int NewCustomers { get; set; }
        public int ReturningCustomers { get; set; }
        public decimal AverageCustomerValue { get; set; }
        public decimal CustomerLifetimeValue { get; set; }
        public List<CustomerSegmentDto> Segments { get; set; } = new List<CustomerSegmentDto>();
    }

    public class CustomerSegmentDto
    {
        public string SegmentName { get; set; } = string.Empty;
        public int CustomerCount { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AverageValue { get; set; }
        public decimal Percentage { get; set; }
    }

    // Financial Reports
    public class FinancialReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCosts { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal OperatingExpenses { get; set; }
        public decimal CommissionPaid { get; set; }
        public decimal PromotionDiscounts { get; set; }
        public List<RevenueTrendDto> RevenueTrend { get; set; } = new List<RevenueTrendDto>();
    }

    public class RevenueTrendDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public decimal Costs { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }
    }

    // Performance Reports
    public class PerformanceReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageDealSize { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal CustomerSatisfaction { get; set; }
        public int TotalLeads { get; set; }
        public int QualifiedLeads { get; set; }
        public decimal LeadConversionRate { get; set; }
        public List<SalesTargetPerformanceDto> TargetPerformance { get; set; } = new List<SalesTargetPerformanceDto>();
    }

    public class SalesTargetPerformanceDto
    {
        public Guid TargetId { get; set; }
        public string TargetName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string DealerName { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal AchievementAmount { get; set; }
        public decimal AchievementPercentage { get; set; }
        public int TargetQuantity { get; set; }
        public int AchievementQuantity { get; set; }
        public decimal QuantityAchievementPercentage { get; set; }
        public bool IsAchieved { get; set; }
        public DateTime TargetEndDate { get; set; }
        public int DaysRemaining { get; set; }
    }

    // Dashboard Data
    public class DashboardDataDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        // Key Metrics
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public decimal AverageOrderValue { get; set; }
        
        // Growth Metrics
        public decimal RevenueGrowth { get; set; }
        public decimal OrderGrowth { get; set; }
        public decimal CustomerGrowth { get; set; }
        
        // Charts Data
        public List<SalesTrendDto> SalesChart { get; set; } = new List<SalesTrendDto>();
        public List<TopSellerDto> TopSellers { get; set; } = new List<TopSellerDto>();
        public List<TopVehicleDto> TopVehicles { get; set; } = new List<TopVehicleDto>();
        public List<CustomerSegmentDto> CustomerSegments { get; set; } = new List<CustomerSegmentDto>();
        
        // Performance Indicators
        public decimal ConversionRate { get; set; }
        public decimal CustomerSatisfaction { get; set; }
        public int PendingOrders { get; set; }
        public int OverdueFeedbacks { get; set; }
        public decimal SalesTargetAchievement { get; set; }
    }

    // Export Options
    public class ExportOptionsDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Format { get; set; } = "excel"; // excel, pdf, csv
        public Guid? DealerId { get; set; }
        public Guid? UserId { get; set; }
        public List<string> IncludeCharts { get; set; } = new List<string>();
        public bool IncludeDetails { get; set; } = true;
    }
}
