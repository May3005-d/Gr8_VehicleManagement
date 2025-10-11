using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Customers
{
    public class DetailsModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public DetailsModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public CustomerDetailDto? Customer { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanCreateOrder { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public int TestDriveCount { get; set; }
        public int InteractionCount { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var result = await _customerService.GetCustomerDetailsAsync(id);
            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            Customer = result.Data;

            TotalOrders = Customer.Orders.Count;
            TotalSpent = Customer.TotalSpent;
            TestDriveCount = Customer.TestDrives.Count;
            InteractionCount = Customer.RecentInteractions.Count;

            var userType = HttpContext.Session.GetUserType();
            CanEdit = userType == UserType.Admin || userType == UserType.EVMStaff ||
                      userType == UserType.DealerManager || userType == UserType.DealerStaff;
            CanDelete = userType == UserType.Admin || userType == UserType.EVMStaff;
            CanCreateOrder = CanEdit;

            return Page();
        }
    }
}

