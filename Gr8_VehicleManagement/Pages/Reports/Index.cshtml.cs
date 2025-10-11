using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Common.Helpers;
using Gr8_VehicleManagement.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly IReportingService _reportingService;
        private readonly ICustomerService _customerService;

        public IndexModel(IReportingService reportingService, ICustomerService customerService)
        {
            _reportingService = reportingService;
            _customerService = customerService;
        }

        public DashboardDataDto DashboardData { get; set; } = new DashboardDataDto();
        public List<CustomerDto> Dealers { get; set; } = new List<CustomerDto>(); // Using CustomerDto as placeholder
        public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime EndDate { get; set; } = DateTime.Now;
        public Guid? SelectedDealerId { get; set; }
        public bool CanViewReports { get; set; }

        public async Task<IActionResult> OnGetAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            Guid? dealerId = null)
        {
            // Check permissions
            var userType = HttpContext.Session.GetUserType();
            CanViewReports = userType == UserType.Admin || userType == UserType.EVMStaff || 
                           userType == UserType.DealerManager || userType == UserType.DealerStaff;

            if (!CanViewReports)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToPage("/AccessDenied");
            }

            // Set date range
            StartDate = startDate ?? DateTime.Now.AddMonths(-1);
            EndDate = endDate ?? DateTime.Now;
            SelectedDealerId = dealerId;

            try
            {
                // Get dealers for filter - placeholder implementation
                Dealers = new List<CustomerDto>(); // Placeholder - would need proper dealer service

                // Get dashboard data
                var dashboardResult = await _reportingService.GetDashboardDataAsync(
                    StartDate, EndDate, SelectedDealerId);

                if (dashboardResult.Success && dashboardResult.Data != null)
                {
                    DashboardData = dashboardResult.Data;
                }
                else
                {
                    // Initialize with default values if no data
                    DashboardData = new DashboardDataDto
                    {
                        StartDate = StartDate,
                        EndDate = EndDate,
                        TotalOrders = 0,
                        TotalRevenue = 0,
                        TotalCustomers = 0,
                        AverageOrderValue = 0,
                        RevenueGrowth = 0,
                        OrderGrowth = 0,
                        CustomerGrowth = 0,
                        SalesChart = new List<SalesTrendDto>(),
                        TopSellers = new List<TopSellerDto>(),
                        TopVehicles = new List<TopVehicleDto>(),
                        CustomerSegments = new List<CustomerSegmentDto>(),
                        ConversionRate = 0,
                        CustomerSatisfaction = 0,
                        PendingOrders = 0,
                        OverdueFeedbacks = 0,
                        SalesTargetAchievement = 0
                    };
                }

                return Page();
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error loading reports: {ex.Message}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải báo cáo.";
                return Page();
            }
        }
    }
}
