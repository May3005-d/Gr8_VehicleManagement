using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly IOrderService _orderService;

        public DetailsModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public OrderDetailDto? Order { get; set; }
        public bool CanEdit { get; set; }
        public bool CanCancel { get; set; }
        public bool CanDeliver { get; set; }
        public bool CanCreatePayment { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public int PaymentCount { get; set; }
        public int FeedbackCount { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            Order = result.Data;

            TotalPaid = Order.Payments.Where(p => p.Status == PaymentStatus.PaidInFull).Sum(p => p.Amount);
            RemainingAmount = Order.TotalAmount - TotalPaid;
            PaymentCount = Order.Payments.Count;
            FeedbackCount = Order.Feedbacks.Count;

            var userType = HttpContext.Session.GetUserType();
            CanEdit = userType == UserType.Admin || userType == UserType.EVMStaff ||
                      userType == UserType.DealerManager || userType == UserType.DealerStaff;
            CanCancel = CanEdit && Order.Status != OrderStatus.Delivered && Order.Status != OrderStatus.Cancelled;
            CanDeliver = CanEdit && Order.Status == OrderStatus.Processing;
            CanCreatePayment = CanEdit && Order.Status != OrderStatus.Cancelled;

            return Page();
        }
    }
}

