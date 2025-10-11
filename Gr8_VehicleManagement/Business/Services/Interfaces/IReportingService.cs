using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Data.Enums;

namespace Gr8_VehicleManagement.Business.Services.Interfaces
{
    public interface IReportingService
    {
        // Sales Reports
        Task<ServiceResponse<SalesReportDto>> GetSalesReportAsync(
            DateTime startDate, 
            DateTime endDate, 
            Guid? dealerId = null, 
            Guid? userId = null);

        Task<ServiceResponse<List<SalesTrendDto>>> GetSalesTrendAsync(
            DateTime startDate, 
            DateTime endDate, 
            string period = "monthly", // daily, weekly, monthly, yearly
            Guid? dealerId = null);

        Task<ServiceResponse<List<TopSellerDto>>> GetTopSellersAsync(
            DateTime startDate, 
            DateTime endDate, 
            int count = 10,
            Guid? dealerId = null);

        Task<ServiceResponse<List<TopVehicleDto>>> GetTopVehiclesAsync(
            DateTime startDate, 
            DateTime endDate, 
            int count = 10,
            Guid? dealerId = null);

        // Customer Reports
        Task<ServiceResponse<CustomerReportDto>> GetCustomerReportAsync(
            DateTime startDate, 
            DateTime endDate, 
            Guid? dealerId = null);

        Task<ServiceResponse<List<CustomerSegmentDto>>> GetCustomerSegmentsAsync(
            DateTime startDate, 
            DateTime endDate, 
            Guid? dealerId = null);

        // Financial Reports
        Task<ServiceResponse<FinancialReportDto>> GetFinancialReportAsync(
            DateTime startDate, 
            DateTime endDate, 
            Guid? dealerId = null);

        Task<ServiceResponse<List<RevenueTrendDto>>> GetRevenueTrendAsync(
            DateTime startDate, 
            DateTime endDate, 
            string period = "monthly",
            Guid? dealerId = null);

        // Performance Reports
        Task<ServiceResponse<PerformanceReportDto>> GetPerformanceReportAsync(
            DateTime startDate, 
            DateTime endDate, 
            Guid? dealerId = null, 
            Guid? userId = null);

        Task<ServiceResponse<List<SalesTargetPerformanceDto>>> GetSalesTargetPerformanceAsync(
            DateTime startDate, 
            DateTime endDate, 
            Guid? dealerId = null, 
            Guid? userId = null);

        // Export Functions
        Task<ServiceResponse<byte[]>> ExportSalesReportAsync(
            DateTime startDate, 
            DateTime endDate, 
            string format = "excel", // excel, pdf, csv
            Guid? dealerId = null);

        Task<ServiceResponse<byte[]>> ExportCustomerReportAsync(
            DateTime startDate, 
            DateTime endDate, 
            string format = "excel",
            Guid? dealerId = null);

        // Dashboard Data
        Task<ServiceResponse<DashboardDataDto>> GetDashboardDataAsync(
            DateTime startDate, 
            DateTime endDate, 
            Guid? dealerId = null, 
            Guid? userId = null);
    }
}
