using Gr8_VehicleManagement.Data;
using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Pages.Dashboard
{
    public class AdminModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserDto? CurrentUser { get; set; }
        public int TotalDealers { get; set; }
        public int TotalVehicleModels { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check authentication
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login");
            }

            // Check authorization - Only Admin and EVMStaff can access
            var userType = HttpContext.Session.GetUserType();
            if (userType != UserType.Admin && userType != UserType.EVMStaff)
            {
                return RedirectToPage("/Error", new { message = "Bạn không có quyền truy cập trang này" });
            }

            CurrentUser = HttpContext.Session.GetUserSession();

            // Load statistics
            TotalDealers = await _context.Dealers.CountAsync();
            TotalVehicleModels = await _context.VehicleModels.CountAsync();
            TotalOrders = await _context.Orders.CountAsync();
            TotalCustomers = await _context.Customers.CountAsync();

            return Page();
        }
    }
}

