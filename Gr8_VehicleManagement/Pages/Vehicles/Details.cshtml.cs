using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Vehicles
{
    public class DetailsModel : PageModel
    {
        private readonly IVehicleService _vehicleService;

        public DetailsModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public VehicleModelDto? Vehicle { get; set; }
        public List<VehicleVersionDto> Versions { get; set; } = new List<VehicleVersionDto>();
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            // Get vehicle model
            var result = await _vehicleService.GetModelByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            Vehicle = result.Data;

            // Get versions
            var versionsResult = await _vehicleService.GetVersionsByModelIdAsync(id);
            if (versionsResult.Success && versionsResult.Data != null)
            {
                Versions = versionsResult.Data;
            }

            // Check permissions
            var userType = HttpContext.Session.GetUserType();
            CanEdit = userType == UserType.Admin || userType == UserType.EVMStaff;
            CanDelete = userType == UserType.Admin || userType == UserType.EVMStaff;

            return Page();
        }

        public Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            // Check permissions
            var userType = HttpContext.Session.GetUserType();
            if (userType != UserType.Admin && userType != UserType.EVMStaff)
            {
                return Task.FromResult<IActionResult>(Forbid());
            }

            // TODO: Implement delete logic
            TempData["Message"] = "Chức năng xóa đang phát triển";
            return Task.FromResult<IActionResult>(RedirectToPage("/Vehicles/Index"));
        }
    }
}

