using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Orders
{
    public class EditModel : PageModel
    {
        private readonly IOrderService _orderService;

        public EditModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public OrderDetailDto? Order { get; set; }

        [BindProperty]
        public UpdateOrderInput Input { get; set; } = new UpdateOrderInput();

        public class UpdateOrderInput
        {
            public Guid Id { get; set; }
            public OrderStatus Status { get; set; }
            public PaymentStatus PaymentStatus { get; set; }
            public DateTime? ExpectedDeliveryDate { get; set; }
            public DateTime? ActualDeliveryDate { get; set; }
            public string? ContractNumber { get; set; }
            public DateTime? ContractSignedAt { get; set; }
            public DateTime? DeliveredAt { get; set; }
            public string? DeliveryNotes { get; set; }
            public string? CancellationReason { get; set; }
            public string? Notes { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var userType = HttpContext.Session.GetUserType();
            if (userType != UserType.Admin && userType != UserType.EVMStaff &&
                userType != UserType.DealerManager && userType != UserType.DealerStaff)
            {
                return Forbid();
            }

            var result = await _orderService.GetOrderByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            Order = result.Data;

            // Initialize input with current values
            Input.Id = Order.Id;
            Input.Status = Order.Status;
            Input.PaymentStatus = Order.PaymentStatus;
            Input.ExpectedDeliveryDate = Order.ExpectedDeliveryDate;
            Input.ActualDeliveryDate = Order.ActualDeliveryDate;
            Input.ContractNumber = Order.ContractNumber;
            Input.ContractSignedAt = Order.ContractSignedAt;
            Input.DeliveredAt = Order.DeliveredAt;
            Input.DeliveryNotes = Order.DeliveryNotes;
            Input.CancellationReason = Order.CancellationReason;
            Input.Notes = Order.Notes;

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
                // Reload order data
                var result = await _orderService.GetOrderByIdAsync(Input.Id);
                if (result.Success && result.Data != null)
                {
                    Order = result.Data;
                }
                return Page();
            }

            try
            {
                var updateDto = new UpdateOrderDto
                {
                    Id = Input.Id,
                    Status = Input.Status,
                    PaymentStatus = Input.PaymentStatus,
                    ExpectedDeliveryDate = Input.ExpectedDeliveryDate,
                    ActualDeliveryDate = Input.ActualDeliveryDate,
                    ContractNumber = Input.ContractNumber,
                    ContractSignedAt = Input.ContractSignedAt,
                    DeliveredAt = Input.DeliveredAt,
                    DeliveryNotes = Input.DeliveryNotes,
                    CancellationReason = Input.CancellationReason,
                    Notes = Input.Notes
                };

                var result = await _orderService.UpdateOrderAsync(updateDto);
                if (result.Success)
                {
                    TempData["Message"] = "Cập nhật đơn hàng thành công!";
                    return RedirectToPage("/Orders/Details", new { id = Input.Id });
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                    
                    // Reload order data
                    var orderResult = await _orderService.GetOrderByIdAsync(Input.Id);
                    if (orderResult.Success && orderResult.Data != null)
                    {
                        Order = orderResult.Data;
                    }
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật đơn hàng");
                
                // Reload order data
                var orderResult = await _orderService.GetOrderByIdAsync(Input.Id);
                if (orderResult.Success && orderResult.Data != null)
                {
                    Order = orderResult.Data;
                }
                return Page();
            }
        }
    }
}
