using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Payments
{
    public class DetailsModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public DetailsModel(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        public PaymentDetailDto? Payment { get; set; }
        public decimal TotalPaidForOrder { get; set; }
        public decimal RemainingOrderAmount { get; set; }
        public int PaymentCountForOrder { get; set; }
        public bool CanEdit { get; set; }
        public bool CanRefund { get; set; }
        public bool CanApprove { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var userType = HttpContext.Session.GetUserType();
            CanEdit = userType == UserType.Admin || userType == UserType.EVMStaff ||
                      userType == UserType.DealerManager || userType == UserType.DealerStaff;
            CanRefund = CanEdit;
            CanApprove = CanEdit;

            // Get payment details
            var result = await _paymentService.GetPaymentByIdAsync(id.Value);
            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            Payment = result.Data;

            // Get order statistics
            var orderResult = await _orderService.GetOrderByIdAsync(Payment.OrderId);
            if (orderResult.Success && orderResult.Data != null)
            {
                TotalPaidForOrder = orderResult.Data.Payments
                    .Where(p => p.Status == PaymentStatus.PaidInFull)
                    .Sum(p => p.Amount);
                RemainingOrderAmount = orderResult.Data.TotalAmount - TotalPaidForOrder;
                PaymentCountForOrder = orderResult.Data.Payments.Count;
            }

            return Page();
        }
    }
}
