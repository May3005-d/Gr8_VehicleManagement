using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Gr8_VehicleManagement.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Promotions
{
    public class IndexModel : PageModel
    {
        private readonly IPromotionService _promotionService;

        public IndexModel(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        public List<PromotionDto> Promotions { get; set; } = new List<PromotionDto>();
        public int TotalPromotions { get; set; }
        public int ActivePromotions { get; set; }
        public int ExpiringSoon { get; set; }
        public decimal TotalDiscount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(TotalPromotions / (double)PageSize);

        // Filters
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? IsActive { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }

        public async Task<IActionResult> OnGetAsync(int? pageNumber)
        {
            try
            {
                // Check authorization
                var userType = HttpContext.Session.GetUserType();
                CanCreate = userType == UserType.Admin || userType == UserType.EVMStaff;
                CanEdit = userType == UserType.Admin || userType == UserType.EVMStaff;

                // Set page number
                if (pageNumber.HasValue && pageNumber.Value > 0)
                {
                    PageNumber = pageNumber.Value;
                }

                // Get promotions
                var result = await _promotionService.GetAllPromotionsAsync(
                    searchTerm: SearchTerm,
                    isActive: IsActive,
                    startDate: StartDate,
                    endDate: EndDate,
                    pageNumber: PageNumber,
                    pageSize: PageSize);

                if (result.Success)
                {
                    Promotions = result.Data ?? new List<PromotionDto>();
                }

                // Get statistics
                await LoadStatistics();

                return Page();
            }
            catch (Exception ex)
            {
                // Log error
                return Page();
            }
        }

        private async Task LoadStatistics()
        {
            try
            {
                // Get total promotions count
                var totalResult = await _promotionService.GetTotalPromotionsCountAsync();
                if (totalResult.Success)
                {
                    TotalPromotions = totalResult.Data;
                }

                // Get active promotions count
                var activeResult = await _promotionService.GetActivePromotionsCountAsync();
                if (activeResult.Success)
                {
                    ActivePromotions = activeResult.Data;
                }

                // Get expiring soon count (within 7 days)
                var expiringDate = DateTime.Now.AddDays(7);
                var expiringResult = await _promotionService.GetAllPromotionsAsync(
                    isActive: true,
                    endDate: expiringDate);
                if (expiringResult.Success)
                {
                    ExpiringSoon = expiringResult.Data?.Count ?? 0;
                }

                // Get total discount given (this month)
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                var discountResult = await _promotionService.GetTotalDiscountGivenAsync(startOfMonth, endOfMonth);
                if (discountResult.Success)
                {
                    TotalDiscount = discountResult.Data;
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the page
            }
        }
    }
}
