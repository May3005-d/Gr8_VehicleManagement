using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Common.Helpers;
using Gr8_VehicleManagement.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Promotions
{
    public class CreateModel : PageModel
    {
        private readonly IPromotionService _promotionService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IPromotionService promotionService, ILogger<CreateModel> logger)
        {
            _promotionService = promotionService;
            _logger = logger;
        }

        [BindProperty]
        public CreatePromotionDto Input { get; set; } = new CreatePromotionDto();

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            // Check authorization
            if (!HttpContext.Session.IsAdmin() && !HttpContext.Session.IsEVMStaff())
            {
                return RedirectToPage("/Account/Login");
            }

            // Set default values
            Input.StartDate = DateTime.Now;
            Input.EndDate = DateTime.Now.AddMonths(1);
            Input.IsActive = true;

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

                // Validate dates
                if (Input.StartDate >= Input.EndDate)
                {
                    ModelState.AddModelError("Input.EndDate", "Ngày kết thúc phải lớn hơn ngày bắt đầu");
                    return Page();
                }

                // Create promotion
                var result = await _promotionService.CreatePromotionAsync(Input);

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    _logger.LogWarning("Failed to create promotion: {Message}", result.Message);
                    return Page();
                }

                _logger.LogInformation("Promotion {PromotionId} created successfully", result.Data!.Id);
                TempData["SuccessMessage"] = "Tạo khuyến mãi thành công!";
                return RedirectToPage("/Promotions/Details", new { id = result.Data.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating promotion");
                ErrorMessage = "Đã xảy ra lỗi khi tạo khuyến mãi. Vui lòng thử lại.";
                return Page();
            }
        }
    }
}
