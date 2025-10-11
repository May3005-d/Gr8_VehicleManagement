using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Payments
{
    public class EditModel : PageModel
    {
        private readonly IPaymentService _paymentService;

        public EditModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [BindProperty]
        public UpdatePaymentDto Payment { get; set; } = new UpdatePaymentDto();
        
        public PaymentDetailDto? PaymentDetails { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var userType = HttpContext.Session.GetUserType();
            if (userType != UserType.Admin && userType != UserType.EVMStaff &&
                userType != UserType.DealerManager && userType != UserType.DealerStaff)
            {
                return Forbid();
            }

            var result = await _paymentService.GetPaymentByIdAsync(id.Value);
            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            PaymentDetails = result.Data;

            // Map to UpdatePaymentDto
            Payment.Id = result.Data.Id;
            Payment.Status = result.Data.Status;
            Payment.TransactionId = result.Data.TransactionId;
            Payment.PaymentProof = result.Data.PaymentProof;
            Payment.LateFee = result.Data.LateFee;
            Payment.RefundReason = result.Data.RefundReason;
            Payment.RefundApprovedAt = result.Data.RefundApprovedAt;
            Payment.Notes = result.Data.Notes;

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
                return Page();
            }

            try
            {
                var result = await _paymentService.UpdatePaymentAsync(Payment);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Cập nhật thanh toán thành công!";
                    return RedirectToPage("/Payments/Details", new { id = Payment.Id });
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật thanh toán");
                return Page();
            }
        }
    }
}
