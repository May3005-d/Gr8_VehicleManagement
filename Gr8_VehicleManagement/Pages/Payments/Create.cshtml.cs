using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Payments
{
    public class CreateModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;

        public CreateModel(IPaymentService paymentService, IOrderService orderService, ICustomerService customerService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _customerService = customerService;
        }

        [BindProperty]
        public CreatePaymentDto Payment { get; set; } = new CreatePaymentDto();

        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public List<CustomerDto> Customers { get; set; } = new List<CustomerDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userType = HttpContext.Session.GetUserType();
            if (userType != UserType.Admin && userType != UserType.EVMStaff &&
                userType != UserType.DealerManager && userType != UserType.DealerStaff)
            {
                return Forbid();
            }

            // Set default values
            Payment.PaymentDate = DateTime.Now;

            // Load orders and customers
            await LoadDataAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userType = HttpContext.Session.GetUserType();
            if (userType != UserType.Admin && userType != UserType.EVMStaff &&
                userType != UserType.DealerManager && userType != UserType.DealerStaff)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            try
            {
                // Set processor
                var userId = HttpContext.Session.GetUserId();
                if (userId.HasValue)
                {
                    Payment.ProcessedBy = userId.Value;
                }

                var result = await _paymentService.CreatePaymentAsync(Payment);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Tạo thanh toán thành công!";
                    return RedirectToPage("/Payments/Details", new { id = result.Data?.Id });
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                    await LoadDataAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo thanh toán");
                await LoadDataAsync();
                return Page();
            }
        }

        private async Task LoadDataAsync()
        {
            // Load orders
            var ordersResult = await _orderService.GetAllOrdersAsync();
            if (ordersResult.Success)
            {
                Orders = ordersResult.Data ?? new List<OrderDto>();
            }

            // Load customers
            var customersResult = await _customerService.GetAllCustomersAsync();
            if (customersResult.Success)
            {
                Customers = customersResult.Data ?? new List<CustomerDto>();
            }
        }
    }
}
