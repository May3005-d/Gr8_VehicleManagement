using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Vehicles
{
    public class EditModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IVehicleService vehicleService, ILogger<EditModel> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        [BindProperty]
        public UpdateVehicleModelDto Input { get; set; } = new UpdateVehicleModelDto();

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public string? ErrorMessage { get; set; }
        public List<VehicleImageDto> CurrentImages { get; set; } = new List<VehicleImageDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Check authorization
                if (!HttpContext.Session.IsAdmin() && !HttpContext.Session.IsEVMStaff())
                {
                    return RedirectToPage("/Account/Login");
                }

                // Get vehicle model details
                var result = await _vehicleService.GetVehicleModelByIdAsync(Id);
                if (!result.Success || result.Data == null)
                {
                    ErrorMessage = "Không tìm thấy mẫu xe này.";
                    return Page();
                }

                // Map to input model
                Input.Id = result.Data.Id;
                Input.Name = result.Data.Name;
                Input.Code = result.Data.Code;
                Input.Category = result.Data.Category;
                Input.Brand = result.Data.Brand;
                Input.Description = result.Data.Description;
                Input.EngineType = result.Data.EngineType;
                Input.BatteryCapacity = result.Data.BatteryCapacity;
                Input.Range = result.Data.Range;
                Input.ChargingTime = result.Data.ChargingTime;
                Input.SeatingCapacity = result.Data.SeatingCapacity;
                Input.Features = result.Data.Features;
                Input.IsActive = result.Data.IsActive;

                // Get current images
                CurrentImages = result.Data.Images ?? new List<VehicleImageDto>();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading vehicle model for edit");
                ErrorMessage = "Đã xảy ra lỗi khi tải thông tin mẫu xe.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Reload current images for display
                    var result = await _vehicleService.GetVehicleModelByIdAsync(Id);
                    if (result.Success && result.Data != null)
                    {
                        CurrentImages = result.Data.Images ?? new List<VehicleImageDto>();
                    }
                    return Page();
                }

                // Check authorization
                if (!HttpContext.Session.IsAdmin() && !HttpContext.Session.IsEVMStaff())
                {
                    return RedirectToPage("/Account/Login");
                }

                // Process uploaded images
                if (Input.Images != null && Input.Images.Any())
                {
                    var imageUrls = new List<string>();
                    foreach (var image in Input.Images)
                    {
                        if (image.Length > 0)
                        {
                            // Save image to wwwroot/images/vehicles/
                            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "vehicles");
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }

                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                            var filePath = Path.Combine(uploadsFolder, fileName);
                            
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }
                            
                            imageUrls.Add($"/images/vehicles/{fileName}");
                        }
                    }
                    Input.NewImageUrls = imageUrls;
                }

                // Update vehicle model
                var updateResult = await _vehicleService.UpdateVehicleModelAsync(Input);

                if (!updateResult.Success)
                {
                    ErrorMessage = updateResult.Message;
                    _logger.LogWarning("Failed to update vehicle model: {Message}", updateResult.Message);
                    
                    // Reload current images for display
                    var result = await _vehicleService.GetVehicleModelByIdAsync(Id);
                    if (result.Success && result.Data != null)
                    {
                        CurrentImages = result.Data.Images ?? new List<VehicleImageDto>();
                    }
                    return Page();
                }

                _logger.LogInformation("Vehicle model {VehicleId} updated successfully", Input.Id);
                TempData["SuccessMessage"] = "Cập nhật mẫu xe thành công!";
                return RedirectToPage("/Vehicles/Details", new { id = Input.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vehicle model");
                ErrorMessage = "Đã xảy ra lỗi khi cập nhật mẫu xe. Vui lòng thử lại.";
                
                // Reload current images for display
                try
                {
                    var result = await _vehicleService.GetVehicleModelByIdAsync(Id);
                    if (result.Success && result.Data != null)
                    {
                        CurrentImages = result.Data.Images ?? new List<VehicleImageDto>();
                    }
                }
                catch
                {
                    // Ignore error loading images
                }
                
                return Page();
            }
        }
    }

}
