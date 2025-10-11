using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Vehicles
{
    public class CreateModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IVehicleService vehicleService, ILogger<CreateModel> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        [BindProperty]
        public CreateVehicleModelDto Input { get; set; } = new CreateVehicleModelDto();

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            // Check authorization
            if (!HttpContext.Session.IsAdmin() && !HttpContext.Session.IsEVMStaff())
            {
                return RedirectToPage("/Account/Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
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
                    Input.ImageUrls = imageUrls;
                }

                // Create vehicle model
                var result = await _vehicleService.CreateVehicleModelAsync(Input);

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    _logger.LogWarning("Failed to create vehicle model: {Message}", result.Message);
                    return Page();
                }

                _logger.LogInformation("Vehicle model {VehicleId} created successfully", result.Data!.Id);
                TempData["SuccessMessage"] = "Tạo mẫu xe thành công!";
                return RedirectToPage("/Vehicles/Details", new { id = result.Data.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vehicle model");
                ErrorMessage = "Đã xảy ra lỗi khi tạo mẫu xe. Vui lòng thử lại.";
                return Page();
            }
        }
    }

}
