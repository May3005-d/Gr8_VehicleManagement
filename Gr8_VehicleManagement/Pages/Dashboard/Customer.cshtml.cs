using Gr8_VehicleManagement.Data;
using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Pages.Dashboard
{
    public class CustomerModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CustomerModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserDto? CurrentUser { get; set; }
        public int MyOrders { get; set; }
        public int PurchasedVehicles { get; set; }
        public int MyFeedbacks { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login");
            }

            var userType = HttpContext.Session.GetUserType();
            if (userType != UserType.Customer)
            {
                return RedirectToPage("/Error", new { message = "Bạn không có quyền truy cập trang này" });
            }

            CurrentUser = HttpContext.Session.GetUserSession();
            var userId = HttpContext.Session.GetUserId();

            // TODO: Customer dashboard statistics
            // Note: Customer entity no longer has UserId direct link
            // Need to implement customer login separately
            
            // Placeholder async operation to avoid CS1998 warning
            await Task.Delay(1);

            return Page();
        }
    }
}

