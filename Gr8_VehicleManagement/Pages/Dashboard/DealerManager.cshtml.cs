using Gr8_VehicleManagement.Data;
using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Pages.Dashboard
{
    public class DealerManagerModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DealerManagerModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserDto? CurrentUser { get; set; }
        public int MonthlyOrders { get; set; }
        public int InventoryCount { get; set; }
        public int TotalCustomers { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login");
            }

            var userType = HttpContext.Session.GetUserType();
            if (userType != UserType.DealerManager && userType != UserType.DealerStaff)
            {
                return RedirectToPage("/Error", new { message = "Bạn không có quyền truy cập trang này" });
            }

            CurrentUser = HttpContext.Session.GetUserSession();
            var dealerId = HttpContext.Session.GetDealerId();

            if (dealerId.HasValue)
            {
                var currentMonth = DateTime.Now.Month;
                MonthlyOrders = await _context.Orders
                    .Where(o => o.DealerId == dealerId.Value && o.OrderDate.Month == currentMonth)
                    .CountAsync();

                InventoryCount = await _context.VehicleInventories
                    .Where(v => v.DealerId == dealerId.Value && v.Status == VehicleStatus.Available)
                    .CountAsync();

                // TODO: Customer no longer has AssignedDealerId
                TotalCustomers = 0;
            }

            return Page();
        }
    }
}

