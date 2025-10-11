using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Common.Helpers;
using Gr8_VehicleManagement.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.SalesTargets
{
    public class IndexModel : PageModel
    {
        private readonly ISalesTargetService _salesTargetService;

        public IndexModel(ISalesTargetService salesTargetService)
        {
            _salesTargetService = salesTargetService;
        }

        public List<SalesTargetDto> SalesTargets { get; set; } = new List<SalesTargetDto>();
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        
        // Statistics
        public int TotalTargets { get; set; }
        public int ActiveTargets { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal AverageAchievement { get; set; }
        
        // Filter properties
        public string? SearchTerm { get; set; }
        public string? TargetFor { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;

        public async Task<IActionResult> OnGetAsync(
            string? searchTerm = null,
            string? targetFor = null,
            bool? isActive = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? pageNumber = null)
        {
            // Get current user type for permissions
            var userType = HttpContext.Session.GetUserType();
            CanCreate = userType == UserType.Admin || userType == UserType.EVMStaff;
            CanEdit = userType == UserType.Admin || userType == UserType.EVMStaff;

            // Set filter properties
            SearchTerm = searchTerm;
            TargetFor = targetFor;
            IsActive = isActive;
            StartDate = startDate;
            EndDate = endDate;
            CurrentPage = pageNumber ?? 1;

            try
            {
                // Parse TargetFor enum
                TargetFor? targetForEnum = null;
                if (!string.IsNullOrEmpty(targetFor) && Enum.TryParse<TargetFor>(targetFor, out var parsedTargetFor))
                {
                    targetForEnum = parsedTargetFor;
                }

                // Get sales targets
                var result = await _salesTargetService.GetAllSalesTargetsAsync(
                    searchTerm: searchTerm,
                    targetFor: targetForEnum,
                    isActive: isActive,
                    startDate: startDate,
                    endDate: endDate,
                    pageNumber: CurrentPage,
                    pageSize: PageSize);

                if (result.Success && result.Data != null)
                {
                    SalesTargets = result.Data;
                }

                // Get statistics
                await LoadStatistics();

                // Calculate pagination
                var totalCountResult = await _salesTargetService.GetTotalSalesTargetsCountAsync();
                if (totalCountResult.Success)
                {
                    TotalPages = (int)Math.Ceiling((double)totalCountResult.Data / PageSize);
                }

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải danh sách chỉ tiêu bán hàng.";
                return Page();
            }
        }

        private async Task LoadStatistics()
        {
            try
            {
                // Get total targets count
                var totalCountResult = await _salesTargetService.GetTotalSalesTargetsCountAsync();
                if (totalCountResult.Success)
                {
                    TotalTargets = totalCountResult.Data;
                }

                // Get active targets count
                var activeCountResult = await _salesTargetService.GetActiveSalesTargetsCountAsync();
                if (activeCountResult.Success)
                {
                    ActiveTargets = activeCountResult.Data;
                }

                // Get total commission
                var commissionResult = await _salesTargetService.GetTotalCommissionEarnedAsync();
                if (commissionResult.Success)
                {
                    TotalCommission = commissionResult.Data;
                }

                // Calculate average achievement
                if (SalesTargets.Any())
                {
                    AverageAchievement = SalesTargets.Average(st => st.AchievementPercentage);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't break the page
                Console.WriteLine($"Error loading statistics: {ex.Message}");
            }
        }
    }
}
