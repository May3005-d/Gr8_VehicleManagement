using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login");
            }

            // Redirect to appropriate dashboard based on user type
            var userType = HttpContext.Session.GetUserType();
            
            return userType switch
            {
                Data.Enums.UserType.Admin => RedirectToPage("/Dashboard/Admin"),
                Data.Enums.UserType.EVMStaff => RedirectToPage("/Dashboard/EVMStaff"),
                Data.Enums.UserType.DealerManager => RedirectToPage("/Dashboard/DealerManager"),
                Data.Enums.UserType.DealerStaff => RedirectToPage("/Dashboard/DealerStaff"),
                Data.Enums.UserType.Customer => RedirectToPage("/Dashboard/Customer"),
                _ => RedirectToPage("/Index")
            };
        }
    }
}

