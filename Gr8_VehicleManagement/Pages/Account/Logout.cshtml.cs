using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(ILogger<LogoutModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            // Show confirmation page
            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                var userId = HttpContext.Session.GetUserId();
                
                if (userId.HasValue)
                {
                    _logger.LogInformation("User {UserId} logged out", userId.Value);
                }

                // Clear session
                HttpContext.Session.ClearUserSession();

                TempData["Message"] = "Bạn đã đăng xuất thành công!";

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi đăng xuất. Vui lòng thử lại.";
                return RedirectToPage("/Index");
            }
        }
    }
}

