using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Vehicles
{
    public class IndexModel : PageModel
    {
        private readonly IVehicleService _vehicleService;

        public IndexModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public List<VehicleModelDto> Vehicles { get; set; } = new List<VehicleModelDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        // Filters
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Category { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        public List<string> Categories { get; set; } = new List<string>();

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }

        public async Task<IActionResult> OnGetAsync(int? pageNumber)
        {
            // Get current user type for permissions
            var userType = HttpContext.Session.GetUserType();
            CanCreate = userType == UserType.Admin || userType == UserType.EVMStaff;
            CanEdit = userType == UserType.Admin || userType == UserType.EVMStaff;

            // Set page number
            if (pageNumber.HasValue && pageNumber.Value > 0)
            {
                PageNumber = pageNumber.Value;
            }

            // Get categories for filter
            var categoriesResult = await _vehicleService.GetCategoriesAsync();
            if (categoriesResult.Success)
            {
                Categories = categoriesResult.Data ?? new List<string>();
            }

            // Get vehicles
            var result = await _vehicleService.GetAllModelsAsync(
                searchTerm: SearchTerm,
                category: Category,
                minPrice: MinPrice,
                maxPrice: MaxPrice,
                isActive: null, // Show all
                pageNumber: PageNumber,
                pageSize: PageSize);

            if (result.Success)
            {
                Vehicles = result.Data ?? new List<VehicleModelDto>();
                
                // Get total count for pagination
                var totalResult = await _vehicleService.GetTotalModelsCountAsync();
                if (totalResult.Success)
                {
                    TotalCount = totalResult.Data;
                }
            }

            return Page();
        }
    }
}

